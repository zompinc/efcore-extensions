namespace Zomp.EFCore.WindowFunctions.Sqlite.Tests;

[Collection(nameof(SqliteCollection))]
public class SqliteSpecificTests(ITestOutputHelper output) : TestBase(output)
{
    /// <summary>
    /// Ensures paging is generated with SQLite's <c>LIMIT</c> rather than ANSI <c>OFFSET ... FETCH</c>.
    /// </summary>
    /// <remarks>
    /// https://github.com/zompinc/efcore-extensions/issues/23. Enabling window functions used to
    /// replace the SQLite query SQL generator with the provider agnostic one, which broke every
    /// query using Take / Skip / First, whether or not it contained a window function.
    /// </remarks>
    [Fact]
    public void Issue23PagingUsesLimit()
    {
        var sql = DbContext.TestRows.OrderBy(r => r.Id).Take(1).ToQueryString();

        Assert.Contains("LIMIT", sql, StringComparison.Ordinal);
        Assert.DoesNotContain("FETCH", sql, StringComparison.Ordinal);
    }
}
