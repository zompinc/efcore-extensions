namespace Zomp.EFCore.BinaryFunctions.Npgsql.Tests;

[Collection(nameof(NpgsqlCollection))]
public class BinaryTests : IDisposable
{
    private readonly NpgsqlTestDbContext dbContext;
    private readonly Testing.BinaryTests binaryTests;

    private bool disposed;

    public BinaryTests(ITestOutputHelper output)
    {
        dbContext = new(output.ToLoggerFactory());
        binaryTests = new(dbContext);
    }

    [Fact(Skip = "Need to query cast(extract(epoch from t.\"Date\") as BigInt)")]
    public void CastDateToByteArray() => binaryTests.CastDateToByteArray();

    [Fact]
    public void CastIntToByteArray() => binaryTests.CastIntToByteArray();

    [Fact]
    public void CastNullableIntToByteArray() => binaryTests.CastNullableIntToByteArray();

    [Fact(Skip = "TODO: convert to bit")]
    public void CastBoolToByteArray() => binaryTests.CastBoolToByteArray();

    [Fact(Skip = "Need to convert UUID to bytea")]
    public void SimpleCastGuid() => binaryTests.SimpleCastGuid();

    [Fact(Skip = "Need to convert UUID to bytea")]
    public void ConcatenateGuidAndInt() => binaryTests.ConcatenateGuidAndInt();

    [Fact]
    public void ConcatenateTwoInts() => binaryTests.ConcatenateTwoInts();

    [Fact(Skip = "Must be able to convert double precision into bit(64) or bytea")]
    public void DoubleConversion() => binaryTests.DoubleConversion();

    [Fact]
    public void BinaryCastFromIntToShort() => binaryTests.BinaryCastFromIntToShort();

    [Fact(Skip = "Find a way to avoid the error: cannot cast type double precision to bit")]
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