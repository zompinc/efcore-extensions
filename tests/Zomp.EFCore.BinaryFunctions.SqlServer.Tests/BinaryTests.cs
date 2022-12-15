namespace Zomp.EFCore.BinaryFunctions.SqlServer.Tests;

[Collection(nameof(SqlServerCollection))]
public class BinaryTests : IDisposable
{
    private readonly SqlServerTestDbContext dbContext;
    private readonly Testing.BinaryTests binaryTests;

    private bool disposed;

    public BinaryTests(ITestOutputHelper output)
    {
        dbContext = new(output.ToLoggerFactory());
        binaryTests = new(dbContext);
    }

    [Fact]
    public void CastDateToByteArray() => binaryTests.CastDateToByteArray();

    [Fact]
    public void CastIntToByteArray() => binaryTests.CastIntToByteArray();

    [Fact]
    public void CastNullableIntToByteArray() => binaryTests.CastNullableIntToByteArray();

    [Fact]
    public void CastBoolToByteArray() => binaryTests.CastBoolToByteArray();

    [Fact]
    public void SimpleCastGuid() => binaryTests.SimpleCastGuid();

    [Fact]
    public void ConcatenateGuidAndInt() => binaryTests.ConcatenateGuidAndInt();

    [Fact]
    public void ConcatenateTwoInts() => binaryTests.ConcatenateTwoInts();

    [Fact]
    public void DoubleConversion() => binaryTests.DoubleConversion();

    [Fact]
    public void BinaryCastFromIntToShort() => binaryTests.BinaryCastFromIntToShort();

    [Fact]
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