namespace Zomp.EFCore.WindowFunctions.Npgsql.Tests;

[Collection(nameof(NpgsqlCollection))]
public class MaxTests : IDisposable
{
    private readonly NpgsqlTestDbContext dbContext;
    private readonly Testing.MaxTests maxTests;

    public MaxTests(ITestOutputHelper output)
    {
        dbContext = new(output.ToLoggerFactory());
        maxTests = new Testing.MaxTests(dbContext);
    }

    [Fact]
    public void SimpleMax() => maxTests.SimpleMax();

    [Fact]
    public void SimpleMaxNullable() => maxTests.SimpleMaxNullable();

    [Fact]
    public void MaxBetweenCurrentRowAndOne() => maxTests.MaxBetweenCurrentRowAndOne();

    [Fact]
    public void MaxBetweenTwoPreceding() => maxTests.MaxBetweenTwoPreceding();

    [Fact]
    public void MaxBetweenTwoFollowing() => maxTests.MaxBetweenTwoFollowing();

    [Fact]
    public void MaxBetweenFollowingAndUnbounded() => maxTests.MaxBetweenFollowingAndUnbounded();

    [Fact]
    public void SimpleMaxWithCast() => maxTests.SimpleMaxWithCast();

    [Fact]
    public void MaxWithCastToString() => maxTests.MaxWithCastToString();

    [Fact]
    public void MaxWithPartition() => maxTests.MaxWithPartition();

    [Fact]
    public void MaxWith2Partitions() => maxTests.MaxWith2Partitions();

    [Fact(Skip = "Can't max over bit(n) or bytea in postgres")]
    public void LastNonNull() => maxTests.LastNonNull();

    [Fact(Skip = "Can't max over bit(n) or bytea in postgres")]
    public void LastNonNullShorthand() => maxTests.LastNonNullShorthand();

    [Fact]
    public void LastNonNullArithmetic() => maxTests.LastNonNullArithmetic();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (dbContext != null)
            {
                dbContext.Dispose();
            }
        }
    }
}