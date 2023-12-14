namespace Zomp.EFCore.BinaryFunctions.Testing;

public class BinaryTests(TestDbContext dbContext) : IDisposable
{
    public void CastDateToByteArray()
    {
        var query = dbContext.TestRows
            .Select(r => EF.Functions.GetBytes(r.Date));

        var result = query.ToList();
    }

    public void CastIntToByteArray()
    {
        var query = dbContext.TestRows
            .Select(r => EF.Functions.GetBytes(r.Id));

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .Select(r => BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(r.Id)));

        Assert.Equal(expectedSequence, result);
    }

    public void CastNullableIntToByteArray()
    {
        var query = dbContext.TestRows
            .Select(r => EF.Functions.GetBytes(r.Col1));

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .Select(r => r.Col1.HasValue ? BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(r.Col1.Value)) : null);

        Assert.Equal(expectedSequence, result);
    }

    public void CastBoolToByteArray()
    {
        var query = dbContext.TestRows
            .Select(r => EF.Functions.GetBytes(r.Id % 3 == 2 ? (bool?)null : r.Id % 3 == 0));

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .Select(r => r.Id % 3 == 2 ? null : BitConverter.GetBytes(r.Id % 3 == 0));

        Assert.Equal(expectedSequence, result);
    }

    public void SimpleCastGuid()
    {
        var query = dbContext.TestRows
            .Select(r => EF.Functions.GetBytes(r.SomeGuid));
        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows.Select(r => r.SomeGuid.ToByteArray());

        Assert.Equal(expectedSequence, result);
    }

    public void ConcatenateGuidAndInt()
    {
        var query = dbContext.TestRows
            .Select(r => EF.Functions.Concat(EF.Functions.GetBytes(r.SomeGuid), EF.Functions.GetBytes(r.Id)));

        var expectedSequence = TestFixture.TestRows
            .Select(r => ReverseEndianAndCombine(r.SomeGuid, r.Id));

        var result = query.ToList();

        Assert.Equal(expectedSequence, result);
    }

    public void ConcatenateTwoInts()
    {
        var query = dbContext.TestRows
            .Where(r => r.Col1.HasValue)
            .Select(r => EF.Functions.Concat(EF.Functions.GetBytes(r.Id), EF.Functions.GetBytes(r.Col1!.Value)));
        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .Where(r => r.Col1.HasValue)
            .Select(r => ReverseEndianAndCombine(r.Id, r.Col1!.Value));

        Assert.Equal(expectedSequence, result);
    }

    public void DoubleConversion()
    {
        var query = dbContext.TestRows
            .Where(r => r.Col1.HasValue)
            .Select(r => EF.Functions.ToValue<double>(EF.Functions.GetBytes(r.Col1!.Value / 2d)));
        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .Where(r => r.Col1.HasValue)
            .Select(r => (double?)r.Col1!.Value / 2d);

        Assert.Equal(expectedSequence, result);
    }

    public void BinaryCastFromIntToShort()
    {
        var shortOverflow = 1 << 16;
        var query = dbContext.TestRows
            .Select(r => EF.Functions.BinaryCast<int, short>(r.Id + shortOverflow));

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .Select(r =>
            {
#if !EF_CORE_7 && !EF_CORE_6
                var @ref = r.Id + shortOverflow;
                return MemoryMarshal.GetReference(MemoryMarshal.Cast<int, short>(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(ref @ref), 1)));
#else
                return MemoryMarshal.GetReference(MemoryMarshal.Cast<int, short>(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(r.Id + shortOverflow), 1)));
#endif
            });

        Assert.Equal(expectedSequence, result);
    }

    public void BinaryCastFromDoubleToLong()
    {
        var shortOverflow = 1 << 16;
        var query = dbContext.TestRows
            .Select(r => EF.Functions.BinaryCast<double, long>((r.Id / 2d) + shortOverflow));

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .Select(r =>
            {
#if !EF_CORE_7 && !EF_CORE_6
                var @ref = (r.Id / 2d) + shortOverflow;
                return MemoryMarshal.GetReference(MemoryMarshal.Cast<double, long>(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(ref @ref), 1)));
#else
                return MemoryMarshal.GetReference(MemoryMarshal.Cast<double, long>(MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef((r.Id / 2d) + shortOverflow), 1)));
#endif
            });

        Assert.Equal(expectedSequence, result);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    private static byte[] ReverseEndianAndCombine(Guid x, int y)
    {
        var bytes = new byte[20];
        x.TryWriteBytes(bytes);
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