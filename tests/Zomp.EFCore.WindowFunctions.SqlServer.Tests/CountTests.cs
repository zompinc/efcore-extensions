namespace Zomp.EFCore.WindowFunctions.SqlServer.Tests;

[Collection(nameof(SqlServerCollection))]
public class CountTests : TestBase
{
    private readonly Testing.CountTests countTests;

    public CountTests(ITestOutputHelper output)
        : base(output)
    {
        countTests = new Testing.CountTests(DbContext);
    }

    [Fact]
    public void CountBasic() => countTests.CountBasic();

    [Fact]
    public void CountBasicNullable() => countTests.CountBasicNullable();

    [Fact]
    public void CountBetweenCurrentRowAndNext() => countTests.CountBetweenCurrentRowAndNext();

    [Fact]
    public void CountBetweenCurrentRowAndNextNullable() => countTests.CountBetweenCurrentRowAndNextNullable();

    [Fact]
    public void CountBetweenTwoPreceding() => countTests.CountBetweenTwoPreceding();

    [Fact]
    public void CountBetweenTwoFollowing() => countTests.CountBetweenTwoFollowing();

    [Fact]
    public void CountBetweenFollowingAndUnbounded() => countTests.CountBetweenFollowingAndUnbounded();

    [Fact]
    public void CountWithPartition() => countTests.CountWithPartition();

    [Fact]
    public void CountWith2Partitions() => countTests.CountWith2Partitions();

    [Fact]
    public void SimpleCountWithCast() => countTests.SimpleCountWithCast();

    [Fact]
    public void CountWithCastToString() => countTests.CountWithCastToString();

    [Fact]
    public void CountBinary() => countTests.CountBinary();
}