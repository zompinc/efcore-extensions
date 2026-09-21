namespace Zomp.EFCore.BinaryFunctions.Testing;

public partial class BinaryTests
{
    [Fact]
    public void CastDateToByteArray()
    {
        var query = DbContext.TestRows
            .Select(r => EF.Functions.GetBytes(r.Date));

        var result = query.ToList();
    }

    [Fact]
    public void CastIntToByteArray()
    {
        var query = DbContext.TestRows
            .Select(r => EF.Functions.GetBytes(r.Id));

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .Select(r => BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(r.Id)));

        Assert.Equal(expectedSequence, result);
    }

    [Fact]
    public void CastNullableIntToByteArray()
    {
        var query = DbContext.TestRows
            .Select(r => EF.Functions.GetBytes(r.Col1));

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .Select(r => r.Col1.HasValue ? BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(r.Col1.Value)) : null);

        Assert.Equal(expectedSequence, result);
    }

    [Fact]
    public void CastBoolToByteArray()
    {
        var query = DbContext.TestRows
            .Select(r => EF.Functions.GetBytes(r.Id % 3 == 2 ? (bool?)null : r.Id % 3 == 0));

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .Select(r => r.Id % 3 == 2 ? null : BitConverter.GetBytes(r.Id % 3 == 0));

        Assert.Equal(expectedSequence, result);
    }

    [Fact]
    public void SimpleCastGuid()
    {
        var query = DbContext.TestRows
            .Select(r => EF.Functions.GetBytes(r.SomeGuid));
        var result = query.ToList();

        // PostgreSQL keeps a uuid in the order it is written. SQL Server and .NET store the first three groups little-endian.
        var expectedSequence = TestFixture.TestRows.Select(r => r.SomeGuid.ToByteArray(bigEndian: DbContext.IsPostgreSQL));

        Assert.Equal(expectedSequence, result);
    }

    [Fact]
    public void ConcatenateGuidAndInt()
    {
        var query = DbContext.TestRows
            .Select(r => EF.Functions.Concat(EF.Functions.GetBytes(r.SomeGuid), EF.Functions.GetBytes(r.Id)));

        var expectedSequence = TestFixture.TestRows
            .Select(r => ReverseEndianAndCombine(r.SomeGuid, r.Id, DbContext.IsPostgreSQL));

        var result = query.ToList();

        Assert.Equal(expectedSequence, result);
    }

    [Fact]
    public void ConcatenateTwoInts()
    {
        var query = DbContext.TestRows
            .Where(r => r.Col1.HasValue)
            .Select(r => EF.Functions.Concat(EF.Functions.GetBytes(r.Id), EF.Functions.GetBytes(r.Col1!.Value)));
        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .Where(r => r.Col1.HasValue)
            .Select(r => ReverseEndianAndCombine(r.Id, r.Col1!.Value));

        Assert.Equal(expectedSequence, result);
    }

    [SkippableFact]
    public void DoubleConversion()
    {
        Skip.If(DbContext.IsPostgreSQL, "Must be able to convert double precision into bit(64) or bytea");

        var query = DbContext.TestRows
            .Where(r => r.Col1.HasValue)
            .Select(r => EF.Functions.ToValue<double>(EF.Functions.GetBytes(r.Col1!.Value / 2d)));
        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .Where(r => r.Col1.HasValue)
            .Select(r => (double?)r.Col1!.Value / 2d);

        Assert.Equal(expectedSequence, result);
    }

    [Fact]
    public void BinaryCastFromIntToShort()
    {
        var shortOverflow = 1 << 16;
        var query = DbContext.TestRows
            .Select(r => EF.Functions.BinaryCast<int, short>(r.Id + shortOverflow));

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .Select(r =>
            {
                var @ref = r.Id + shortOverflow;
                return MemoryMarshal.GetReference(MemoryMarshal.Cast<int, short>(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(ref @ref), 1)));
            });

        Assert.Equal(expectedSequence, result);
    }

    [SkippableFact]
    public void BinaryCastFromDoubleToLong()
    {
        Skip.If(DbContext.IsSqlite, "TODO: implement / drop");

        var shortOverflow = 1 << 16;
        var query = DbContext.TestRows
            .Select(r => EF.Functions.BinaryCast<double, long>((r.Id / 2d) + shortOverflow));

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .Select(r =>
            {
                var @ref = (r.Id / 2d) + shortOverflow;
                return MemoryMarshal.GetReference(MemoryMarshal.Cast<double, long>(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(ref @ref), 1)));
            });

        Assert.Equal(expectedSequence, result);
    }

    private static byte[] ReverseEndianAndCombine(Guid x, int y, bool bigEndianGuid)
    {
        var bytes = new byte[20];
        _ = x.TryWriteBytes(bytes, bigEndianGuid, out _);
        BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan(16), y);
        return bytes;
    }

    private static byte[] ReverseEndianAndCombine(int x, int y)
    {
        var bytes = new byte[8];
        BinaryPrimitives.WriteInt32BigEndian(bytes, x);
        BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan(4), y);
        return bytes;
    }
}