namespace Zomp.EFCore.WindowFunctions;

#nullable enable

/// <summary>
/// Provides extension methods for window functions.
/// </summary>
public static partial class DbFunctionsExtensions
{
    /// <summary>
    /// The NTH_VALUE window function returns the value of the expression at the nth row of the window frame, or NULL when the frame is shorter. SQL Server has no NTH_VALUE. With ORDER BY and no explicit frame, the frame ends at the current row and its peers.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="n">The row of the window frame to return the value from, counting from 1.</param>
    /// <param name="over">over clause.</param>
    /// <returns>NthValue for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? NthValue<T>(this DbFunctions _, T expression, long n, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(NthValue))  + UseWindowFunctions);

    /// <summary>
    /// The NTH_VALUE window function returns the value of the expression at the nth row of the window frame, or NULL when the frame is shorter. SQL Server has no NTH_VALUE. With ORDER BY and no explicit frame, the frame ends at the current row and its peers.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="n">The row of the window frame to return the value from, counting from 1.</param>
    /// <param name="over">over clause.</param>
    /// <returns>NthValue for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? NthValue<T>(this DbFunctions _, T? expression, long n, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(NthValue))  + UseWindowFunctions);

    /// <summary>
    /// The NTH_VALUE window function returns the value of the expression at the nth row of the window frame, or NULL when the frame is shorter. SQL Server has no NTH_VALUE. With ORDER BY and no explicit frame, the frame ends at the current row and its peers.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="n">The row of the window frame to return the value from, counting from 1.</param>
    /// <param name="over">over clause.</param>
    /// <returns>NthValue for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static byte[]? NthValue(this DbFunctions _, byte[]? expression, long n, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(NthValue))  + UseWindowFunctions);

    /// <summary>
    /// The NTH_VALUE window function returns the value of the expression at the nth row of the window frame, or NULL when the frame is shorter. SQL Server has no NTH_VALUE. With ORDER BY and no explicit frame, the frame ends at the current row and its peers.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="n">The row of the window frame to return the value from, counting from 1.</param>
    /// <param name="over">over clause.</param>
    /// <returns>NthValue for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static string? NthValue(this DbFunctions _, string? expression, long n, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(NthValue))  + UseWindowFunctions);
}