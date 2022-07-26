namespace Zomp.EFCore.WindowFunctions.Npgsql.Tests;

[Collection(nameof(NpgsqlCollection))]
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
    public void RankBasic() => rankTests.RankBasic();

    [Fact]
    public void DenseRankBasic() => rankTests.DenseRankBasic();

    [Fact]
    public void PercentRankBasic() => rankTests.PercentRankBasic(true);
}