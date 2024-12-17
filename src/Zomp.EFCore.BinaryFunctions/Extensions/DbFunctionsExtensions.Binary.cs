#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Zomp.EFCore.BinaryFunctions;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides extension methods for functions manipulating binary data.
/// </summary>
public static partial class DbFunctionsExtensions
{
    private const string UseBinaryFunctions = " One possible cause of this error is that '.UseBinaryFunctions()' was not called on the 'DbContextOptionsBuilder' in 'OnConfiguring'.";

    /// <summary>
    /// Returns the specified object value as a byte array.
    /// </summary>
    /// <typeparam name="T">Type of object being converted.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Object to convert to bytes.</param>
    /// <returns>the specified object value as a byte array.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static byte[] GetBytes<T>(this DbFunctions _, T expression)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GetBytes)) + UseBinaryFunctions);

    /// <summary>
    /// Returns the specified object value as a byte array.
    /// </summary>
    /// <typeparam name="T">Type of object being converted.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Object to convert to bytes.</param>
    /// <returns>the specified object value as a byte array.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static byte[]? GetBytes<T>(this DbFunctions _, T? expression)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(GetBytes)) + UseBinaryFunctions);

    /// <summary>
    /// Concatinates two blob objects.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="binaryValue1">First blob to concatenate.</param>
    /// <param name="binaryValue2">Second blob to concatenate.</param>
    /// <returns>Blob that was resulted from concatination.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static byte[]? Concat(this DbFunctions _, byte[]? binaryValue1, byte[]? binaryValue2)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Concat)) + UseBinaryFunctions);

    /// <summary>
    /// Returns part of a binary expression.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="binaryValue">Binary expression.</param>
    /// <param name="start">Start index.</param>
    /// <param name="length">The length.</param>
    /// <returns>Part of a binary expression.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static byte[]? Substring(this DbFunctions _, byte[]? binaryValue, long start, long length)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Substring)) + UseBinaryFunctions);

    /// <summary>
    /// Converts an array of bytes to a value of a specified type.
    /// </summary>
    /// <typeparam name="T">Type to convert to.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="array">Expression that represents the bytes.</param>
    /// <returns>The type.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? ToValue<T>(this DbFunctions _, byte[]? array)
        where T : unmanaged
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(ToValue)) + UseBinaryFunctions);

    /// <summary>
    /// Converts an array of bytes to a value of a specified type.
    /// </summary>
    /// <typeparam name="T">Type to convert to.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="array">Expression that represents the bytes.</param>
    /// <param name="start">Start index.</param>
    /// <returns>The type.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? ToValue<T>(this DbFunctions _, byte[]? array, long start)
        where T : unmanaged
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(ToValue)) + UseBinaryFunctions);

    /// <summary>
    /// Casts from one type to another.
    /// </summary>
    /// <typeparam name="TFrom">Type to cast from.</typeparam>
    /// <typeparam name="TTo">Type to cast to.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression containing the type.</param>
    /// <returns>Expression of the converted type.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    /// <remarks>
    /// When converting from larger type to smaller type, least significant digits are used, and overflow error does not occur.
    /// This method is similar to <see cref="MemoryMarshal.Cast{TFrom, TTo}(ReadOnlySpan{TFrom})"/>.
    /// </remarks>
    public static TTo? BinaryCast<TFrom, TTo>(this DbFunctions _, TFrom? expression)
        where TFrom : unmanaged
        where TTo : unmanaged
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(BinaryCast)) + UseBinaryFunctions);

    /// <summary>
    /// Casts from one type to another.
    /// </summary>
    /// <typeparam name="TFrom">Type to cast from.</typeparam>
    /// <typeparam name="TTo">Type to cast to.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression containing the type.</param>
    /// <returns>Expression of the converted type.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    /// <remarks>
    /// When converting from larger type to smaller type, least significant digits are used, and overflow error does not occur.
    /// This method is similar to <see cref="MemoryMarshal.Cast{TFrom, TTo}(ReadOnlySpan{TFrom})"/>.
    /// </remarks>
    public static TTo BinaryCast<TFrom, TTo>(this DbFunctions _, TFrom expression)
        where TFrom : unmanaged
        where TTo : unmanaged
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(BinaryCast)) + UseBinaryFunctions);
}