namespace Zomp.EFCore.WindowFunctions.Testing;

public partial class RankTests
{
    [Fact]
    public void RowNumberBasic()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(r.Id).PartitionBy(r.Id / 10)));

        var result = query.ToList();

        var expectedSequence = TestRows.GroupBy(r => r.Id / 10)
            .SelectMany(g =>
                g.Select((j, i) => (long)(i + 1)));

        Assert.Equal(expectedSequence, result);
    }

    [SkippableFact]
    public void RowNumberEmptyOver()
    {
        Skip.If(DbContext.IsSqlServer);

        var query = DbContext.TestRows
            .Select(r => EF.Functions.RowNumber(EF.Functions.Over()));

        var result = query.ToList();

        var expectedSequence = TestRows
            .Select((j, i) => (long)(i + 1));

        Assert.Equal(expectedSequence, result);
    }

    [Fact]
    public void RankBasic()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Rank(EF.Functions.Over().OrderBy(r.Id / 10)));

        var result = query.ToList();

        var groups = TestRows.GroupBy(r => r.Id / 10);
        var expectedSequence = TestRows
            .Select(r => r.Id / 10)
            .Select(v => (long)groups
                .Where(g => g.Key < v)
                .Select(g => g.Count())
                .Sum() + 1);

        Assert.Equal(expectedSequence, result);
    }

    [Fact]
    public void DenseRankBasic()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.DenseRank(EF.Functions.Over().OrderBy(r.Id / 10)));

        var result = query.ToList();

        var expectedSequence = TestRows
            .GroupBy(r => r.Id / 10)
            .SelectMany((g, i) => g.Select(j => (long)(i + 1)));

        Assert.Equal(expectedSequence, result);
    }

    [Fact]
    public void PercentRankBasic()
    {
        var nullsLast = DbContext.IsPostgreSQL;

        var query = DbContext.TestRows
        .Select(r => EF.Functions.PercentRank(EF.Functions.Over().OrderBy(r.Col1)));

        var result = query.ToList();

        var groups = TestRows.GroupBy(r => r.Col1);

        var comparer = new NullSensitiveComparer<int>(nullsLast);

        var expectedSequence = TestRows
            .Select(r => r.Col1)
            .OrderBy(x => x, comparer)
            .Select(v => groups
                .Where(g => comparer.Compare(g.Key, v) < 0)
                .Select(g => g.Count())
                .Sum() / (double)(TestRows.Length - 1));

        Assert.Equal(expectedSequence, result.Select(r => r));
    }
}