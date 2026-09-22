namespace Zomp.EFCore.WindowFunctions;

#nullable enable

/// <summary>
/// Provides extension methods for window functions.
/// </summary>
public static partial class DbFunctionsExtensions
{
    /// <summary>
    /// The LAST_VALUE window function returns the value of the expression at the last row of the window frame. With ORDER BY and no explicit frame, the frame ends at the current row and its peers, so pass a frame such as Rows().FromCurrentRow().ToUnbounded() for the last row of the partition.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>LastValue for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? LastValue<T>(this DbFunctions _, T expression, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(LastValue))  + UseWindowFunctions);

    /// <summary>
    /// The LAST_VALUE window function returns the value of the expression at the last row of the window frame. With ORDER BY and no explicit frame, the frame ends at the current row and its peers, so pass a frame such as Rows().FromCurrentRow().ToUnbounded() for the last row of the partition.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>LastValue for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? LastValue<T>(this DbFunctions _, T? expression, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(LastValue))  + UseWindowFunctions);

    /// <summary>
    /// The LAST_VALUE window function returns the value of the expression at the last row of the window frame. With ORDER BY and no explicit frame, the frame ends at the current row and its peers, so pass a frame such as Rows().FromCurrentRow().ToUnbounded() for the last row of the partition.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>LastValue for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static byte[]? LastValue(this DbFunctions _, byte[]? expression, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(LastValue))  + UseWindowFunctions);

    /// <summary>
    /// The LAST_VALUE window function returns the value of the expression at the last row of the window frame. With ORDER BY and no explicit frame, the frame ends at the current row and its peers, so pass a frame such as Rows().FromCurrentRow().ToUnbounded() for the last row of the partition.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>LastValue for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static string? LastValue(this DbFunctions _, string? expression, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(LastValue))  + UseWindowFunctions);
}