namespace Zomp.EFCore.WindowFunctions;

#nullable enable

/// <summary>
/// Provides extension methods for window functions.
/// </summary>
public static partial class DbFunctionsExtensions
{
    /// <summary>
    /// The FIRST_VALUE window function returns the value of the expression at the first row of the window frame.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>FirstValue for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? FirstValue<T>(this DbFunctions _, T expression, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(FirstValue))  + UseWindowFunctions);

    /// <summary>
    /// The FIRST_VALUE window function returns the value of the expression at the first row of the window frame.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>FirstValue for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? FirstValue<T>(this DbFunctions _, T? expression, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(FirstValue))  + UseWindowFunctions);

    /// <summary>
    /// The FIRST_VALUE window function returns the value of the expression at the first row of the window frame.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>FirstValue for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static byte[]? FirstValue(this DbFunctions _, byte[]? expression, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(FirstValue))  + UseWindowFunctions);

    /// <summary>
    /// The FIRST_VALUE window function returns the value of the expression at the first row of the window frame.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>FirstValue for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static string? FirstValue(this DbFunctions _, string? expression, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(FirstValue))  + UseWindowFunctions);
}