namespace Zomp.EFCore.WindowFunctions;

#nullable enable

/// <summary>
/// Provides extension methods for window functions.
/// </summary>
public static partial class DbFunctionsExtensions
{
    /// <summary>
    /// The RANK() window function returns the rank value of the expression across all input values.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>

    /// <param name="over">over clause.</param>
    /// <returns>Rank for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static long Rank(this DbFunctions _, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Rank))  + UseWindowFunctions);
}