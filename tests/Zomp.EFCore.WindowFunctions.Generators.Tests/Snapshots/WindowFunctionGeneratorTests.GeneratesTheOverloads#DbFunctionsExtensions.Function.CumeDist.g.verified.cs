namespace Zomp.EFCore.WindowFunctions;

#nullable enable

/// <summary>
/// Provides extension methods for window functions.
/// </summary>
public static partial class DbFunctionsExtensions
{
    /// <summary>
    /// The CUME_DIST() window function returns the cumulative distribution of the current row: the share of rows in the partition that come before it or are its peers.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>

    /// <param name="over">over clause.</param>
    /// <returns>CumeDist for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static double CumeDist(this DbFunctions _, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(CumeDist))  + UseWindowFunctions);
}