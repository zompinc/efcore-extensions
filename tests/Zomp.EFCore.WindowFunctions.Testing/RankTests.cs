using Zomp.EFCore.WindowFunctions.Query.Internal;

namespace Zomp.EFCore.WindowFunctions.Testing;

public class RankTests
{
    private readonly TestDbContext dbContext;

    public RankTests(TestDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public void RowJoin()
    {
        var query =
            from testRow in dbContext.TestRows
            join testRow2 in
                (from subRow in dbContext.TestRows
                 select new
                 {
                     subRow.Id,
                     RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(subRow.Date).PartitionBy(subRow.Col1)),
                 }).AsSubQuery()
            on testRow.Id equals testRow2.Id
            where testRow.Id > 1 && testRow2.RowNumber <= 2
            select testRow2;

        var result = query.ToQueryString();

        var items = query.ToArray();
    }

    public void RowNumberBasic()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(r.Id).PartitionBy(r.Id / 10)),
        });

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows.GroupBy(r => r.Id / 10)
            .SelectMany(g =>
                g.Select((j, i) => (long)(i + 1)));

        Assert.Equal(expectedSequence, result.Select(r => r.RowNumber));
    }

    public void RowNumberEmptyOver()
    {
        var query = dbContext.TestRows
            .Select(r => new
            {
                RowNumber = EF.Functions.RowNumber(EF.Functions.Over()),
            });

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .Select((j, i) => (long)(i + 1));

        Assert.Equal(expectedSequence, result.Select(r => r.RowNumber));
    }

    public void RankBasic()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Rank = EF.Functions.Rank(EF.Functions.Over().OrderBy(r.Id / 10)),
        });

        var result = query.ToList();

        var groups = TestFixture.TestRows.GroupBy(r => r.Id / 10);
        var expectedSequence = TestFixture.TestRows
            .Select(r => r.Id / 10)
            .Select(v => (long)groups
                .Where(g => g.Key < v)
                .Select(g => g.Count())
                .Sum() + 1);

        Assert.Equal(expectedSequence, result.Select(r => r.Rank));
    }

    public void DenseRankBasic()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            DenseRank = EF.Functions.DenseRank(EF.Functions.Over().OrderBy(r.Id / 10)),
        });

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .GroupBy(r => r.Id / 10)
            .SelectMany((g, i) => g.Select(j => (long)(i + 1)));

        Assert.Equal(expectedSequence, result.Select(r => r.DenseRank));
    }

    public void PercentRankBasic(bool nullsLast = false)
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            PercentRank = EF.Functions.PercentRank(EF.Functions.Over().OrderBy(r.Col1)),
        });

        var result = query.ToList();

        var groups = TestFixture.TestRows.GroupBy(r => r.Col1);

        var comparer = new NullSensitiveComparer<int>(nullsLast);

        var expectedSequence = TestFixture.TestRows
            .Select(r => r.Col1)
            .OrderBy(x => x, comparer)
            .Select(v => groups
                .Where(g => comparer.Compare(g.Key, v) < 0)
                .Select(g => g.Count())
                .Sum() / (double)(TestFixture.TestRows.Length - 1));

        Assert.Equal(expectedSequence, result.Select(r => r.PercentRank));
    }
}