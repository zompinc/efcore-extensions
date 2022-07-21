namespace Zomp.EFCore.WindowFunctions;

/// <summary>
/// Provides extension methods for avg window function.
/// </summary>
public static partial class DbFunctionsExtensions
{
    /// <summary>
    /// The AVG() window function returns the average value of the expression across all input values.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">partition clause.</param>
    /// <returns>Average for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? Avg<T>(this DbFunctions _, T expression, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Avg)));

    /// <summary>
    /// The AVG() window function returns the average value of the expression across all input values.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">partition clause.</param>
    /// <returns>Average for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static T? Avg<T>(this DbFunctions _, T? expression, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Avg)));
}