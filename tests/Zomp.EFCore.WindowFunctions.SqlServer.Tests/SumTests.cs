namespace Zomp.EFCore.WindowFunctions.SqlServer.Tests;

[Collection(nameof(SqlServerCollection))]
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

    [Fact]
    public void SumWithPartitionAndOrder() => sumTests.SumWithPartitionAndOrder();
}