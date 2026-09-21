namespace Zomp.EFCore.WindowFunctions.Testing;

public abstract partial class AvgTests<TResult>
    where TResult : IConvertible
{
    [Test]
    public async Task AvgBasic()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Avg<int, TResult>(r.Id, EF.Functions.Over()));

        var result = query.ToList().Distinct();

        await Assert.That(result).HasSingleItem();

        var expected = ExpectedAverage(TestRows, r => r.Id);

        await Assert.That(Math.Round(result.Single()!.ToDecimal(null), 10)).IsEqualTo(Math.Round(expected, 10));
    }

    [Test]
    public async Task AvgWithPartition()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Avg<int, TResult>(
                r.Id,
                EF.Functions.Over().PartitionBy(r.Id / 10)));

        var result = query.ToList();

        var groups = TestRows.GroupBy(r => r.Id / 10)
            .ToDictionary(r => r.Key, r => ExpectedAverage(r, s => s.Id));

        var expectedSequence = TestRows.Select(r => (decimal?)groups[r.Id / 10]);
        await Assert.That(result.Select(r => r?.ToDecimal(null))).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task AvgDoubleWithPartition()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Avg(
                (double)r.Id,
                EF.Functions.Over().PartitionBy(r.Id / 10)));

        var result = query.ToList();

        var groups = TestRows.GroupBy(r => r.Id / 10)
            .ToDictionary(r => r.Key, r => r.Average(s => s.Id));

        var expectedSequence = TestRows.Select(r => (double?)groups[r.Id / 10]);
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task AvgNullableWithPartition()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Avg(
                (double?)r.Col1,
                EF.Functions.Over().PartitionBy(r.Id / 10)));

        var result = query.ToList();

        var groups = TestRows.GroupBy(r => r.Id / 10)
            .ToDictionary(r => r.Key, r => r.Average(s => s.Col1));

        var expectedSequence = TestRows.Select(r => groups[r.Id / 10]);
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    /// <remarks>
    /// Postgres returns decimal for average, while Sql Server and Sqlite return int.
    /// </remarks>
    private static decimal ExpectedAverage<T>(IEnumerable<T> source, Func<T, int> func)
        => typeof(TResult) == typeof(decimal)
            ? source.Average(z => (decimal)func(z))
            : (int)source.Average(z => func(z));
}
