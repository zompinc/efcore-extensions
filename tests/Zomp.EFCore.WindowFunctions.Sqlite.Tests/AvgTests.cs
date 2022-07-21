namespace Zomp.EFCore.WindowFunctions.Sqlite.Tests;

[Collection(nameof(SqliteCollection))]
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
}