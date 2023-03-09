namespace Zomp.EFCore.WindowFunctions.Sqlite.Tests;

[Collection(nameof(SqliteCollection))]
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
    public void DenseRankBasic() => rankTests.DenseRankBasic();

    [Fact]
    public void PercentRankBasic() => rankTests.PercentRankBasic();
}