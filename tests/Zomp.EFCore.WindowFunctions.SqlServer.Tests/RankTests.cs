namespace Zomp.EFCore.WindowFunctions.SqlServer.Tests;

[Collection(nameof(SqlServerCollection))]
public class RankTests : TestBase
{
    private readonly Testing.RankTests rankTests;

    public RankTests(ITestOutputHelper output)
        : base(output)
    {
        rankTests = new Testing.RankTests(DbContext);
    }

    [Fact]
    public void RowNumberBasic() => rankTests.RowNumberBasic();

    [Fact]
    public void RowNumberEmptyOver() => rankTests.RowNumberEmptyOver();

    [Fact]
    public void RankBasic() => rankTests.RankBasic();

    [Fact]
    public void RankEmptyOver() => rankTests.RankEmptyOver();

    [Fact]
    public void DenseRankBasic() => rankTests.DenseRankBasic();

    [Fact]
    public void DenseRankEmptyOver() => rankTests.DenseRankEmptyOver();

    [Fact]
    public void PercentRankBasic() => rankTests.PercentRankBasic(true);

    [Fact]
    public void PercentRankEmptyOver() => rankTests.PercentRankEmptyOver(true);
}