namespace Zomp.EFCore.BinaryFunctions.Storage.Internal;

/// <summary>
/// Represents a type to be converted into fixed length byte array.
/// </summary>
/// <typeparam name="T">A type to be converted into bytes.</typeparam>
/// <remarks>
/// An example of such type is binary(n) for SQL server or bit(n) for Postgres.
/// </remarks>
public class FixedByteArray<T>
    where T : unmanaged
{
}