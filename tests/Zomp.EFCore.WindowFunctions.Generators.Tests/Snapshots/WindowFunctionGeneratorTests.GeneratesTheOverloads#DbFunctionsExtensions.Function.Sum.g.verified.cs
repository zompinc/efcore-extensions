namespace Zomp.EFCore.WindowFunctions;

#nullable enable

/// <summary>
/// Provides extension methods for window functions.
/// </summary>
public static partial class DbFunctionsExtensions
{
    /// <summary>
    /// The SUM() window function returns the sum value of the expression across all input values.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Sum for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? Sum<T>(this DbFunctions _, T expression, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Sum))  + UseWindowFunctions);

    /// <summary>
    /// The SUM() window function returns the sum value of the expression across all input values.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Sum for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? Sum<T>(this DbFunctions _, T? expression, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Sum))  + UseWindowFunctions);

    /// <summary>
    /// The SUM() window function returns the sum value of the expression across all input values.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <typeparam name="TResult">Type of the result object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Sum for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static TResult? Sum<T, TResult>(this DbFunctions _, T expression, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Sum))  + UseWindowFunctions);

    /// <summary>
    /// The SUM() window function returns the sum value of the expression across all input values.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <typeparam name="TResult">Type of the result object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Sum for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static TResult? Sum<T, TResult>(this DbFunctions _, T? expression, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Sum))  + UseWindowFunctions);
}