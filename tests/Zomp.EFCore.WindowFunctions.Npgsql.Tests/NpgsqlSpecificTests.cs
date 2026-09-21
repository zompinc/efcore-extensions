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
}
