namespace Zomp.EFCore.WindowFunctions.Testing;

public abstract partial class SumTests<TResult>
        where TResult : IConvertible
{
    [Fact]
    public void SimpleSum()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Sum<int, TResult>(r.Id, EF.Functions.Over()));

        var result = query.ToList();

        var sumId = TestRows.Sum(r => r.Id);
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => (long?)sumId);
        Assert.Equal(expectedSequence, result.Select(r => r?.ToInt64(null)));
    }

    [Fact]
    public void SumWithPartition()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Sum<int, TResult>(
                r.Id,
                EF.Functions.Over().PartitionBy(r.Id / 10)));

        var result = query.ToList();

        var groups = TestRows.GroupBy(r => r.Id / 10)
            .ToDictionary(r => r.Key, r => r.Sum(s => s.Id));

        var expectedSequence = TestRows.Select(r => (long?)groups[r.Id / 10]);
        Assert.Equal(expectedSequence, result.Select(r => r?.ToInt64(null)));
    }

    [Fact]
    public void SumWithPartitionAndOrder()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Sum<int, TResult>(
                r.Id,
                EF.Functions.Over().OrderBy(r.Id).PartitionBy(r.Id / 10)));

        var result = query.ToList();

        var groups = TestRows.GroupBy(r => r.Id / 10);

        var expectedSequence = TestRows
            .Select(r => groups
                .Where(g => g.Key == r.Id / 10)
                .SelectMany(g => g)
                .Where(z => z.Id <= r.Id)
                .Sum(s => (long)s.Id));

        Assert.Equal(expectedSequence, result.Select(r => r!.ToInt64(null)));
    }
}
