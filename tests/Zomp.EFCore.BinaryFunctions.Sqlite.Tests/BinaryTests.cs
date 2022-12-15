namespace Zomp.EFCore.BinaryFunctions.Sqlite.Tests;

[Collection(nameof(SqliteCollection))]
public class BinaryTests : IDisposable
{
    private readonly SqliteTestDbContext dbContext;
    private readonly Testing.BinaryTests binaryTests;

    private bool disposed;

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

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                dbContext.Dispose();
                binaryTests.Dispose();
            }

            disposed = true;
        }
    }
}