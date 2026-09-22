using Zomp.EFCore.WindowFunctions.Testing;

namespace Zomp.EFCore.WindowFunctions.Sqlite.Tests;

public class SqliteSpecificTests : TestBase
{
    /// <summary>
    /// Ensures paging is generated with SQLite's <c>LIMIT</c> rather than ANSI <c>OFFSET ... FETCH</c>.
    /// </summary>
    /// <remarks>
    /// https://github.com/zompinc/efcore-extensions/issues/23. Enabling window functions used to
    /// replace the SQLite query SQL generator with the provider agnostic one, which broke every
    /// query using Take / Skip / First, whether or not it contained a window function.
    /// </remarks>
    /// <returns>A task that completes when the SQL has been checked.</returns>
    [Test]
    public async Task Issue23PagingUsesLimit()
    {
        var sql = DbContext.TestRows.OrderBy(r => r.Id).Take(1).ToQueryString();

        await Assert.That(sql).Contains("LIMIT", StringComparison.Ordinal);
        await Assert.That(sql).DoesNotContain("FETCH", StringComparison.Ordinal);
    }

    /// <summary>
    /// SQLite has no standard deviation or variance, and by default says so rather than approximating them.
    /// </summary>
    /// <returns>A task that completes when the exception has been checked.</returns>
    [Test]
    public async Task StandardDeviationIsNotSupportedByDefault()
    {
        var query = DbContext.TestRows
            .Select(r => EF.Functions.StandardDeviationSample(r.Col1, EF.Functions.Over()));

        await Assert.That(() => query.ToList()).Throws<InvalidOperationException>().WithMessageContaining("approximateStandardDeviationAndVariance", StringComparison.Ordinal);
    }

    [Test]
    public async Task StandardDeviationSampleApproximated()
    {
        using var context = new SqliteTestDbContext { ApproximateStandardDeviationAndVariance = true };
        var query = context.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.StandardDeviationSample(r.Col1, EF.Functions.Over().PartitionBy(r.Id / 10)));

        var result = query.ToList();

        await StatisticsTests.AssertMatches(result, values => StatisticsTests.Sqrt(StatisticsTests.SampleVariance(values)));

        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task StandardDeviationPopulationApproximated()
    {
        using var context = new SqliteTestDbContext { ApproximateStandardDeviationAndVariance = true };
        var query = context.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.StandardDeviationPopulation(r.Col1, EF.Functions.Over().PartitionBy(r.Id / 10)));

        var result = query.ToList();

        await StatisticsTests.AssertMatches(result, values => StatisticsTests.Sqrt(StatisticsTests.PopulationVariance(values)));

        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task VarianceSampleApproximated()
    {
        using var context = new SqliteTestDbContext { ApproximateStandardDeviationAndVariance = true };
        var query = context.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.VarianceSample(r.Col1, EF.Functions.Over().PartitionBy(r.Id / 10)));

        var result = query.ToList();

        await StatisticsTests.AssertMatches(result, StatisticsTests.SampleVariance);

        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task VariancePopulationApproximated()
    {
        using var context = new SqliteTestDbContext { ApproximateStandardDeviationAndVariance = true };
        var query = context.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.VariancePopulation(r.Col1, EF.Functions.Over().PartitionBy(r.Id / 10)));

        var result = query.ToList();

        await StatisticsTests.AssertMatches(result, StatisticsTests.PopulationVariance);

        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task StandardDeviationApproximatedOverRowsFrame()
    {
        using var context = new SqliteTestDbContext { ApproximateStandardDeviationAndVariance = true };
        var query = context.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.StandardDeviationPopulation(r.Id, EF.Functions.Over().OrderBy(r.Id).Rows().FromPreceding(1).ToCurrentRow()));

        var result = query.ToList();

        var ordered = TestRows.OrderBy(r => r.Id).Select(r => (int?)r.Id).ToArray();
        var expected = ordered.Select((_, i) => StatisticsTests.Sqrt(StatisticsTests.PopulationVariance(ordered[Math.Max(0, i - 1)..(i + 1)])));

        await Assert.That(result.Select(StatisticsTests.Round)).IsEquivalentTo(expected.Select(StatisticsTests.Round), CollectionOrdering.Matching);

        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task VariancePopulationApproximatedWithWhere()
    {
        using var context = new SqliteTestDbContext { ApproximateStandardDeviationAndVariance = true };
        var query = context.TestRows
            .Where(r => EF.Functions.VariancePopulation(r.Col1, EF.Functions.Over().PartitionBy(r.Id / 10)) > 0)
            .OrderBy(r => r.Id)
            .Select(r => r.Id);

        var result = query.ToList();

        var expected = TestRows
            .Where(r => StatisticsTests.PopulationVariance(TestRows.Where(t => t.Id / 10 == r.Id / 10).Select(t => t.Col1)) > 0)
            .OrderBy(r => r.Id)
            .Select(r => r.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);

        await Verify(query.ToQueryString());
    }

    /// <summary>
    /// A compiled query is cached per internal service provider, so the option must be part of what decides whether
    /// two contexts share one. Otherwise the approximated query would be reused where the option is off.
    /// </summary>
    /// <returns>A task that completes when the exception has been checked.</returns>
    [Test]
    public async Task ApproximationDoesNotLeakIntoContextsWithoutIt()
    {
        using (var approximating = new SqliteTestDbContext { ApproximateStandardDeviationAndVariance = true })
        {
            _ = approximating.TestRows.Select(r => EF.Functions.VarianceSample(r.Col1, EF.Functions.Over())).ToList();
        }

        var query = DbContext.TestRows.Select(r => EF.Functions.VarianceSample(r.Col1, EF.Functions.Over()));

        await Assert.That(() => query.ToList()).Throws<InvalidOperationException>();
    }
}
