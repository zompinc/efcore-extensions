namespace Zomp.EFCore.WindowFunctions.Sqlite.Tests;

public class SqliteSpecificTests : TestBase
{
    /// <summary>
    /// Ensures paging is generated with SQLite's <c>LIMIT</c> rather than ANSI <c>OFFSET ... FETCH</c>.
    /// </summary>
    /// <remarks>
    /// https://github.com/zompinc/efcore-extensions/issues/23. Enabling window functions used to
    /// replace the SQLite query SQL generator with the provider agnostic one, which broke every
    /// query using Take / Skip / First, whether or not it contained a window function.
    /// </remarks>
    /// <returns>A task that completes when the SQL has been checked.</returns>
    [Test]
    public async Task Issue23PagingUsesLimit()
    {
        var sql = DbContext.TestRows.OrderBy(r => r.Id).Take(1).ToQueryString();

        await Assert.That(sql).Contains("LIMIT", StringComparison.Ordinal);
        await Assert.That(sql).DoesNotContain("FETCH", StringComparison.Ordinal);
    }

    /// <summary>
    /// SQLite has no standard deviation or variance, and by default says so rather than approximating them.
    /// </summary>
    /// <returns>A task that completes when the exception has been checked.</returns>
    [Test]
    public async Task StandardDeviationIsNotSupportedByDefault()
    {
        var query = DbContext.TestRows
            .Select(r => EF.Functions.StandardDeviationSample(r.Col1, EF.Functions.Over()));

        await Assert.That(() => query.ToList()).Throws<InvalidOperationException>().WithMessageContaining("approximateStandardDeviationAndVariance", StringComparison.Ordinal);
    }
}
