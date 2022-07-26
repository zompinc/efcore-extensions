namespace Zomp.EFCore.WindowFunctions;

/// <summary>
/// Provides extension methods for rank window function.
/// </summary>
public static partial class DbFunctionsExtensions
{
    /// <summary>
    /// The ROW_NUMBER() window function returns the row number value of the expression across all input values.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="over">partition clause.</param>
    /// <returns>ROW_NUMBER for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static long RowNumber(this DbFunctions _, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(RowNumber)));

    /// <summary>
    /// The RANK() window function returns the rank value of the expression across all input values.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="over">partition clause.</param>
    /// <returns>Rank for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static long Rank(this DbFunctions _, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Rank)));

    /// <summary>
    /// The DENSE_RANK() window function returns the rank value of the expression across all input values.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="over">partition clause.</param>
    /// <returns>Dense rank for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static long DenseRank(this DbFunctions _, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(DenseRank)));

    /// <summary>
    /// The PERCENT_RANK() window function returns the rank value of the expression across all input values.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="over">partition clause.</param>
    /// <returns>Percent rank for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static double PercentRank(this DbFunctions _, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(PercentRank)));
}