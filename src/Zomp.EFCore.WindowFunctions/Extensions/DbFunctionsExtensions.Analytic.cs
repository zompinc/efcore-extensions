namespace Zomp.EFCore.WindowFunctions;

/// <inheritdoc cref="DbFunctionsExtensions"/>
public static partial class DbFunctionsExtensions
{
    /// <summary>
    /// The LEAD() window function.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="default">The default.</param>
    /// <param name="over">partition clause.</param>
    /// <returns>Lead bla.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? Lead<T>(this DbFunctions _, T? expression, long? offset, T @default, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lead)));

    /// <summary>
    /// The LEAD() window function.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="default">The default.</param>
    /// <param name="over">partition clause.</param>
    /// <returns>Lead bla.</returns>
    public static T? Lead<T>(this DbFunctions _, T expression, long? offset, T @default, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lead)));
}
