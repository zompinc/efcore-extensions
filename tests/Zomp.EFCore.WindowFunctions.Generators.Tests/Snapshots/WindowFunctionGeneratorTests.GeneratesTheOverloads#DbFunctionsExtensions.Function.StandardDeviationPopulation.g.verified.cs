namespace Zomp.EFCore.WindowFunctions;

#nullable enable

/// <summary>
/// Provides extension methods for window functions.
/// </summary>
public static partial class DbFunctionsExtensions
{
    /// <summary>
    /// The population standard deviation window function (STDEVP on SQL Server, STDDEV_POP elsewhere) returns the population standard deviation of the expression across all input values.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>StandardDeviationPopulation for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static double? StandardDeviationPopulation<T>(this DbFunctions _, T expression, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(StandardDeviationPopulation))  + UseWindowFunctions);

    /// <summary>
    /// The population standard deviation window function (STDEVP on SQL Server, STDDEV_POP elsewhere) returns the population standard deviation of the expression across all input values.
    /// </summary>
    /// <typeparam name="T">Type of object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Expression to run window function on.</param>
    /// <param name="over">over clause.</param>
    /// <returns>StandardDeviationPopulation for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static double? StandardDeviationPopulation<T>(this DbFunctions _, T? expression, OverClause over)
        where T : struct
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(StandardDeviationPopulation))  + UseWindowFunctions);
}