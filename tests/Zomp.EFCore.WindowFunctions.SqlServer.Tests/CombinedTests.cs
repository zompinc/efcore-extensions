namespace Zomp.EFCore.WindowFunctions.SqlServer.Tests;

[Collection(nameof(SqlServerCollection))]
public class CombinedTests : TestBase
{
    private readonly Testing.CombinedTests combinedTests;

    public CombinedTests(ITestOutputHelper output)
        : base(output)
    {
        combinedTests = new Testing.CombinedTests(DbContext);
    }

    [Fact]
    public void LastNonNull() => combinedTests.LastNonNull();

    [Fact]
    public void LastNonNullShorthand() => combinedTests.LastNonNullShorthand();

    [Fact]
    public void LastNonNullArithmetic() => combinedTests.LastNonNullArithmetic();
}
