namespace Zomp.EFCore.WindowFunctions.Sqlite.Tests;

[Collection(nameof(SqliteCollection))]
public class CombinedTests : TestBase
{
    private readonly Testing.CombinedTests combinedTests;

    public CombinedTests(ITestOutputHelper output)
        : base(output)
    {
        combinedTests = new Testing.CombinedTests(DbContext);
    }

    [Fact(Skip = "Depends on byte concatenation, which SQLite doesn't support out of the box")]
    public void LastNonNull() => combinedTests.LastNonNull();

    [Fact(Skip = "Depends on byte concatenation, which SQLite doesn't support out of the box")]
    public void LastNonNullShorthand() => combinedTests.LastNonNullShorthand();

    [Fact]
    public void LastNonNullArithmetic() => combinedTests.LastNonNullArithmetic();
}
