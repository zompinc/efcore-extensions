namespace Zomp.EFCore.WindowFunctions.SqlServer.Tests;

[Collection(nameof(SqlServerCollection))]
public class AvgTests : TestBase
{
    private readonly Testing.AvgTests sumTests;

    public AvgTests(ITestOutputHelper output)
        : base(output)
    {
        sumTests = new Testing.AvgTests(DbContext);
    }

    [Fact]
    public void AvgBasic() => sumTests.AvgBasic();

    [Fact]
    public void AvgWithPartition() => sumTests.AvgWithPartition();

    [Fact]
    public void AvgDoubleWithPartition() => sumTests.AvgDoubleWithPartition();

    [Fact]
    public void AvgNullableWithPartition() => sumTests.AvgNullableWithPartition();
}