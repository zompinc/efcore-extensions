namespace Zomp.EFCore.WindowFunctions.Testing;

public partial class WhereTests
{
    [Fact]
    public void RowNumberWithWhere()
    {
        var query = DbContext.TestRows
            .Where(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) == 1);

        var result = query.ToList();

        var expected = TestRows.First();

        Assert.Equal(expected, result.Single(), TestRowEqualityComparer.Default);
    }

    [Fact]
    public void DenseRankWithWhere()
    {
        var query = DbContext.TestRows
            .Where(t => EF.Functions.DenseRank(EF.Functions.Over().OrderBy(t.Id / 10)) == 2);

        var result = query.ToList();

        var expectedSequence = TestRows.Where(t => t.Id / 10 == 1);

        Assert.Equal(expectedSequence, result, TestRowEqualityComparer.Default);
    }

    [Fact]
    public void MaxWithWhere()
    {
        var query = DbContext.TestRows
            .Where(t => EF.Functions.Max(t.Id, EF.Functions.Over().PartitionBy(t.Id / 10)) == 23);

        var result = query.ToList();

        var expected = TestRows.Last();

        Assert.Equal(expected, result.Single(), TestRowEqualityComparer.Default);
    }
}
