namespace Zomp.EFCore.WindowFunctions.Testing;

/// <summary>
/// Standard deviation and variance, https://github.com/zompinc/efcore-extensions/issues/17.
/// </summary>
/// <remarks>
/// Partitioned by Id / 10, the Col1 values are {10, -1}, {-12} and {1759}, so the sample functions also
/// meet a partition with a single value, where they are NULL.
/// </remarks>
public partial class StatisticsTests
{
    private const string NotOnSqlite = "SQLite has no standard deviation or variance; SqliteSpecificTests covers it";

    [Test]
    public async Task StandardDeviationSampleWithPartition()
    {
        Skip.When(DbContext.IsSqlite, NotOnSqlite);

        var result = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.StandardDeviationSample(r.Col1, EF.Functions.Over().PartitionBy(r.Id / 10)))
            .ToList();

        await AssertMatches(result, values => Sqrt(SampleVariance(values)));
    }

    [Test]
    public async Task StandardDeviationPopulationWithPartition()
    {
        Skip.When(DbContext.IsSqlite, NotOnSqlite);

        var result = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.StandardDeviationPopulation(r.Col1, EF.Functions.Over().PartitionBy(r.Id / 10)))
            .ToList();

        await AssertMatches(result, values => Sqrt(PopulationVariance(values)));
    }

    [Test]
    public async Task VarianceSampleWithPartition()
    {
        Skip.When(DbContext.IsSqlite, NotOnSqlite);

        var result = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.VarianceSample(r.Col1, EF.Functions.Over().PartitionBy(r.Id / 10)))
            .ToList();

        await AssertMatches(result, SampleVariance);
    }

    [Test]
    public async Task VariancePopulationWithPartition()
    {
        Skip.When(DbContext.IsSqlite, NotOnSqlite);

        var result = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.VariancePopulation(r.Col1, EF.Functions.Over().PartitionBy(r.Id / 10)))
            .ToList();

        await AssertMatches(result, PopulationVariance);
    }

    [Test]
    public async Task StandardDeviationPopulationOverRowsFrame()
    {
        Skip.When(DbContext.IsSqlite, NotOnSqlite);

        var result = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.StandardDeviationPopulation(r.Id, EF.Functions.Over().OrderBy(r.Id).Rows().FromPreceding(1).ToCurrentRow()))
            .ToList();

        var ordered = TestRows.OrderBy(r => r.Id).Select(r => (int?)r.Id).ToArray();
        var expected = ordered.Select((_, i) => Sqrt(PopulationVariance(ordered[Math.Max(0, i - 1)..(i + 1)])));

        await Assert.That(result.Select(Round)).IsEquivalentTo(expected.Select(Round), CollectionOrdering.Matching);
    }

    [Test]
    public async Task VariancePopulationWithWhere()
    {
        Skip.When(DbContext.IsSqlite, NotOnSqlite);

        // The partitions of Col1 are {10, -1}, {-12} and {1759}; only the first varies.
        var result = DbContext.TestRows
            .Where(r => EF.Functions.VariancePopulation(r.Col1, EF.Functions.Over().PartitionBy(r.Id / 10)) > 0)
            .OrderBy(r => r.Id)
            .Select(r => r.Id)
            .ToList();

        var expected = TestRows
            .Where(r => PopulationVariance(TestRows.Where(t => t.Id / 10 == r.Id / 10).Select(t => t.Col1)) > 0)
            .OrderBy(r => r.Id)
            .Select(r => r.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MaxOverStandardDeviation()
    {
        Skip.When(DbContext.IsSqlite, NotOnSqlite);

        var result = DbContext.TestRows
            .Select(r => EF.Functions.StandardDeviationPopulation(r.Col1, EF.Functions.Over().PartitionBy(r.Id / 10)))
            .Max();

        var expected = TestRows
            .Select(r => Sqrt(PopulationVariance(TestRows.Where(t => t.Id / 10 == r.Id / 10).Select(t => t.Col1))))
            .Max();

        await Assert.That(Round(result)).IsEqualTo(Round(expected));
    }

    internal static double? SampleVariance(IEnumerable<int?> values)
    {
        var present = values.Where(v => v.HasValue).Select(v => (double)v!.Value).ToArray();
        if (present.Length < 2)
        {
            return null;
        }

        var mean = present.Average();
        return present.Sum(v => (v - mean) * (v - mean)) / (present.Length - 1);
    }

    internal static double? PopulationVariance(IEnumerable<int?> values)
    {
        var present = values.Where(v => v.HasValue).Select(v => (double)v!.Value).ToArray();
        if (present.Length == 0)
        {
            return null;
        }

        var mean = present.Average();
        return present.Sum(v => (v - mean) * (v - mean)) / present.Length;
    }

    internal static double? Sqrt(double? value) => value is { } v ? Math.Sqrt(v) : null;

    // The databases differ in the last digits.
    internal static double? Round(double? value) => value is { } v ? Math.Round(v, 8) : null;

    /// <summary>
    /// Checks the result of a function partitioned by Id / 10 on Col1, in the order of Id.
    /// </summary>
    internal static async Task AssertMatches(IEnumerable<double?> result, Func<IEnumerable<int?>, double?> function)
    {
        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => function(TestRows.Where(t => t.Id / 10 == r.Id / 10).Select(t => t.Col1)));

        await Assert.That(result.Select(Round)).IsEquivalentTo(expected.Select(Round), CollectionOrdering.Matching);
    }
}
