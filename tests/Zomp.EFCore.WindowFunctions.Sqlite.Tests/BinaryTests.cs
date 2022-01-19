namespace Zomp.EFCore.WindowFunctions.Sqlite.Tests;

[Collection(nameof(SqliteCollection))]
public class BinaryTests
{
    private readonly SqliteTestDbContext dbContext;
    private readonly Testing.BinaryTests binaryTests;

    public BinaryTests(ITestOutputHelper output)
    {
        dbContext = new(output.ToLoggerFactory());
        binaryTests = new(dbContext);
    }

    [Fact]
    public void CastDateToByteArray() => binaryTests.CastDateToByteArray();

    [Fact(Skip = "Investigate why INTEGER returns as text")]
    public void CastIntToByteArray() => binaryTests.CastIntToByteArray();

    [Fact(Skip = "Investigate why INTEGER returns as text")]
    public void CastNullableIntToByteArray() => binaryTests.CastNullableIntToByteArray();

    [Fact(Skip = "Gets stored as text")]
    public void CastBoolToByteArray() => binaryTests.CastBoolToByteArray();

    [Fact]
    public void SimpleCastGuid() => binaryTests.SimpleCastGuid();

    [Fact(Skip = "SQLite has no built-in mechanism to concatenate blobs. https://stackoverflow.com/a/45611692")]
    public void ConcatenateGuidAndInt() => binaryTests.ConcatenateGuidAndInt();

    [Fact(Skip = "SQLite has no built-in mechanism to concatenate blobs. https://stackoverflow.com/a/45611692")]
    public void ConcatenateTwoInts() => binaryTests.ConcatenateTwoInts();

    [Fact]
    public void DoubleConversion() => binaryTests.DoubleConversion();

    [Fact]
    public void BinaryCastFromIntToShort() => binaryTests.BinaryCastFromIntToShort();

    [Fact(Skip = "TODO: implement / drop")]
    public void BinaryCastFromDoubleToLong() => binaryTests.BinaryCastFromDoubleToLong();
}