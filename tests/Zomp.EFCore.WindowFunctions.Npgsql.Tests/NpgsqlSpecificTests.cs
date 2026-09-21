namespace Zomp.EFCore.WindowFunctions.Npgsql.Tests;

public class NpgsqlSpecificTests : TestBase
{
    [Test]
    public async Task Issue12BrokenILike()
    {
        var query = DbContext.TestRows
            .Where(e => EF.Functions.ILike(e.Col1!.ToString()!, "%2%"));

        var result = query.ToList();

        var expected = TestRows.Where(t => t.Col1?.ToString()?.Contains('2', StringComparison.OrdinalIgnoreCase) ?? false);

        await Assert.That(result).IsEquivalentTo(expected, TestRowEqualityComparer.Default, CollectionOrdering.Matching);
    }

    /// <summary>
    /// Npgsql's full-text Rank shares its name with the window function, https://github.com/zompinc/efcore-extensions/issues/30.
    /// </summary>
    /// <returns>A task that completes when the SQL has been checked.</returns>
    [Test]
    public async Task Issue30FullTextRankIsNotTheWindowFunction()
    {
        var query = DbContext.TestRows
            .Select(r => EF.Functions.ToTsVector(r.Id.ToString()).Rank(EF.Functions.ToTsQuery("1")));

        await Assert.That(query.ToQueryString()).Contains("ts_rank(", StringComparison.Ordinal);
        await Assert.That(query.ToList()).Count().IsEqualTo(TestRows.Length);
    }
}
