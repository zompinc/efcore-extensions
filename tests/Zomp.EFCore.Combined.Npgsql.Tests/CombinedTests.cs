namespace Zomp.EFCore.Combined.Npgsql.Tests;

[Collection(nameof(NpgsqlCollection))]
public class CombinedTests : TestBase
{
    private readonly Testing.CombinedTests combinedTests;

    public CombinedTests(ITestOutputHelper output)
        : base(output)
    {
        combinedTests = new Testing.CombinedTests(DbContext);
    }

    [Fact(Skip = "Can't max over bit(n) or bytea in postgres")]
    public void LastNonNull() => combinedTests.LastNonNull();

    [Fact(Skip = "Can't max over bit(n) or bytea in postgres")]
    public void LastNonNullShorthand() => combinedTests.LastNonNullShorthand();

    [Fact]
    public void LastNonNullArithmetic() => combinedTests.LastNonNullArithmetic();
}
