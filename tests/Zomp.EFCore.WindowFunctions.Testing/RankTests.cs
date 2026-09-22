namespace Zomp.EFCore.WindowFunctions.Testing;

public partial class RankTests
{
    [Test]
    public async Task RowNumberBasic()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(r.Id).PartitionBy(r.Id / 10)));

        var result = query.ToList();

        var expectedSequence = TestRows.GroupBy(r => r.Id / 10)
            .SelectMany(g =>
                g.Select((j, i) => (long)(i + 1)));

        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task RowNumberEmptyOver()
    {
        Skip.When(DbContext.IsSqlServer, "SQL Server requires ORDER BY for ROW_NUMBER");

        var query = DbContext.TestRows
            .Select(r => EF.Functions.RowNumber(EF.Functions.Over()));

        var result = query.ToList();

        var expectedSequence = TestRows
            .Select((j, i) => (long)(i + 1));

        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task RankBasic()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Rank(EF.Functions.Over().OrderBy(r.Id / 10)));

        var result = query.ToList();

        var groups = TestRows.GroupBy(r => r.Id / 10);
        var expectedSequence = TestRows
            .Select(r => r.Id / 10)
            .Select(v => (long)groups
                .Where(g => g.Key < v)
                .Sum(g => g.Count()) + 1);

        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task DenseRankBasic()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.DenseRank(EF.Functions.Over().OrderBy(r.Id / 10)));

        var result = query.ToList();

        var expectedSequence = TestRows
            .GroupBy(r => r.Id / 10)
            .SelectMany((g, i) => g.Select(j => (long)(i + 1)));

        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task PercentRankBasic()
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
                .Sum(g => g.Count()) / (double)(TestRows.Length - 1));

        await Assert.That(result.Select(r => r)).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task NTileBasic()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.NTile(3, EF.Functions.Over().OrderBy(r.Id)));

        var result = query.ToList();

        await Assert.That(result).IsEquivalentTo(NTiles(TestRows.Length, 3), CollectionOrdering.Matching);
    }

    [Test]
    public async Task NTileWithPartitionAndVariable()
    {
        var buckets = 2;
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.NTile(buckets, EF.Functions.Over().PartitionBy(r.Id / 10).OrderBy(r.Id)));

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => NTiles(g.Count(), buckets));

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task NTileWithWhere()
    {
        var query = DbContext.TestRows
            .Where(r => EF.Functions.NTile(2, EF.Functions.Over().OrderBy(r.Id)) == 1)
            .OrderBy(r => r.Id)
            .Select(r => r.Id);

        var result = query.ToList();

        var ordered = TestRows.OrderBy(r => r.Id).ToArray();
        var expected = NTiles(ordered.Length, 2).Zip(ordered).Where(p => p.First == 1).Select(p => p.Second.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task CumeDistBasic()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.CumeDist(EF.Functions.Over().OrderBy(r.Id / 10)));

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => (double)TestRows.Count(t => t.Id / 10 <= r.Id / 10) / TestRows.Length);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task CumeDistEmptyOver()
    {
        Skip.When(DbContext.IsSqlServer, "SQL Server requires ORDER BY for CUME_DIST");

        // Without ORDER BY every row is a peer of every other, so the distribution is 1 throughout.
        var query = DbContext.TestRows
            .Select(r => EF.Functions.CumeDist(EF.Functions.Over()));

        var result = query.ToList();

        await Assert.That(result).IsEquivalentTo(TestRows.Select(_ => 1.0), CollectionOrdering.Matching);
    }

    /// <summary>
    /// The buckets NTILE gives to <paramref name="count"/> ordered rows: as equal as possible, the earlier ones one larger.
    /// </summary>
    private static IEnumerable<long> NTiles(int count, int buckets)
    {
        var size = count / buckets;
        var larger = count % buckets;
        return Enumerable.Range(0, count)
            .Select(i => i < larger * (size + 1) ? (i / (size + 1)) + 1L : larger + ((i - (larger * (size + 1))) / size) + 1L);
    }
}
