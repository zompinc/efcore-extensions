namespace Zomp.EFCore.WindowFunctions;

/// <summary>
/// Provides extension methods for COUNT window function.
/// </summary>
public static partial class DbFunctionsExtensions
{
    /// <summary>
    /// The COUNT() window function returns the count of the expression across all input values.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <typeparam name="TResult">Type of the result object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">partition clause.</param>
    /// <returns>Count for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static TResult Count<T, TResult>(this DbFunctions _, T? expression, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Count)));

    /// <summary>
    /// The COUNT() window function returns the count of the expression across all input values.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">partition clause.</param>
    /// <returns>Count for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static int Count<T>(this DbFunctions _, T? expression, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Count)));
}