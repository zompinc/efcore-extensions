namespace Zomp.EFCore.WindowFunctions;

#nullable enable

/// <summary>
/// Provides extension methods for window functions.
/// </summary>
public static partial class DbFunctionsExtensions
{
    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="default">The default.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? Lag<T>(this DbFunctions _, T expression, long offset, T? @default, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="default">The default.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? Lag<T>(this DbFunctions _, T? expression, long offset, T? @default, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="default">The default.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static byte[]? Lag(this DbFunctions _, byte[]? expression, long offset, byte[]? @default, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="default">The default.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static string? Lag(this DbFunctions _, string? expression, long offset, string? @default, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="default">The default.</param>
    /// <param name="nullHandling">Respect nulls or ignore nulls. If omitted or <see langword="null" /> is specified, provider's default is used which is to respect nulls.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? Lag<T>(this DbFunctions _, T expression, long offset, T? @default, NullHandling? nullHandling, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="default">The default.</param>
    /// <param name="nullHandling">Respect nulls or ignore nulls. If omitted or <see langword="null" /> is specified, provider's default is used which is to respect nulls.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? Lag<T>(this DbFunctions _, T? expression, long offset, T? @default, NullHandling? nullHandling, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="default">The default.</param>
    /// <param name="nullHandling">Respect nulls or ignore nulls. If omitted or <see langword="null" /> is specified, provider's default is used which is to respect nulls.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static byte[]? Lag(this DbFunctions _, byte[]? expression, long offset, byte[]? @default, NullHandling? nullHandling, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="default">The default.</param>
    /// <param name="nullHandling">Respect nulls or ignore nulls. If omitted or <see langword="null" /> is specified, provider's default is used which is to respect nulls.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static string? Lag(this DbFunctions _, string? expression, long offset, string? @default, NullHandling? nullHandling, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? Lag<T>(this DbFunctions _, T expression, long offset, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? Lag<T>(this DbFunctions _, T? expression, long offset, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static byte[]? Lag(this DbFunctions _, byte[]? expression, long offset, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static string? Lag(this DbFunctions _, string? expression, long offset, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="nullHandling">Respect nulls or ignore nulls. If omitted or <see langword="null" /> is specified, provider's default is used which is to respect nulls.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? Lag<T>(this DbFunctions _, T expression, long offset, NullHandling? nullHandling, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="nullHandling">Respect nulls or ignore nulls. If omitted or <see langword="null" /> is specified, provider's default is used which is to respect nulls.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? Lag<T>(this DbFunctions _, T? expression, long offset, NullHandling? nullHandling, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="nullHandling">Respect nulls or ignore nulls. If omitted or <see langword="null" /> is specified, provider's default is used which is to respect nulls.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static byte[]? Lag(this DbFunctions _, byte[]? expression, long offset, NullHandling? nullHandling, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="nullHandling">Respect nulls or ignore nulls. If omitted or <see langword="null" /> is specified, provider's default is used which is to respect nulls.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static string? Lag(this DbFunctions _, string? expression, long offset, NullHandling? nullHandling, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? Lag<T>(this DbFunctions _, T expression, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? Lag<T>(this DbFunctions _, T? expression, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static byte[]? Lag(this DbFunctions _, byte[]? expression, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static string? Lag(this DbFunctions _, string? expression, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="nullHandling">Respect nulls or ignore nulls. If omitted or <see langword="null" /> is specified, provider's default is used which is to respect nulls.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? Lag<T>(this DbFunctions _, T expression, NullHandling? nullHandling, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="nullHandling">Respect nulls or ignore nulls. If omitted or <see langword="null" /> is specified, provider's default is used which is to respect nulls.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? Lag<T>(this DbFunctions _, T? expression, NullHandling? nullHandling, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="nullHandling">Respect nulls or ignore nulls. If omitted or <see langword="null" /> is specified, provider's default is used which is to respect nulls.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static byte[]? Lag(this DbFunctions _, byte[]? expression, NullHandling? nullHandling, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);

    /// <summary>
    /// The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="nullHandling">Respect nulls or ignore nulls. If omitted or <see langword="null" /> is specified, provider's default is used which is to respect nulls.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Lag for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static string? Lag(this DbFunctions _, string? expression, NullHandling? nullHandling, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Lag))  + UseWindowFunctions);
}