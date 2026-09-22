namespace Zomp.EFCore.WindowFunctions;

#nullable enable

/// <summary>
/// Provides extension methods for window functions.
/// </summary>
public static partial class DbFunctionsExtensions
{
    /// <summary>
    /// The ROW_NUMBER() window function returns the row number value of the expression across all input values.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>

    /// <param name="over">over clause.</param>
    /// <returns>RowNumber for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static long RowNumber(this DbFunctions _, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(RowNumber))  + UseWindowFunctions);
}