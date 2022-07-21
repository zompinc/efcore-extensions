namespace Zomp.EFCore.WindowFunctions.Sqlite.Tests;

[Collection(nameof(SqliteCollection))]
public class SumTests : TestBase
{
    private readonly Testing.SumTests sumTests;

    public SumTests(ITestOutputHelper output)
        : base(output)
    {
        sumTests = new Testing.SumTests(DbContext);
    }

    [Fact]
    public void SimpleSum() => sumTests.SimpleSum();

    [Fact]
    public void SumWithPartition() => sumTests.SumWithPartition();
}