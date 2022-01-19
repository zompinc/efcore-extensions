namespace Zomp.EFCore.WindowFunctions.Clauses;

/// <summary>
/// Over clause.
/// </summary>
[SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable", Justification = "Only used for expression trees.")]
public class OverClause
{
    private protected OverClause()
    {
    }

    /// <summary>
    /// Gets the lone instance of the OverClause.
    /// </summary>
    internal static OverClause Instance { get; } = new();
}