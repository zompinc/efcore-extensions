namespace Zomp.EFCore.WindowFunctions.Npgsql.Tests;

[Collection(nameof(NpgsqlCollection))]
public class NpgsqlSpecificTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public void Issue12BrokenILike()
    {
        var query = DbContext.TestRows
            .Where(e => EF.Functions.ILike(e.Col1!.ToString()!, "%2%"));

        var result = query.ToList();

        var expected = TestRows.Where(t => t.Col1?.ToString()?.Contains('2', StringComparison.OrdinalIgnoreCase) ?? false);

        Assert.Equal(expected, result, TestRowEqualityComparer.Default);
    }
}
