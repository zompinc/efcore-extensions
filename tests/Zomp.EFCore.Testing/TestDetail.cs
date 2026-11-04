namespace Zomp.EFCore.Testing;

/// <summary>
/// A row with a reference navigation, for queries that read a navigation after a window function.
/// </summary>
public record TestDetail(int Id, int RowId, string Name)
{
    /// <summary>
    /// Gets or sets the row this detail belongs to.
    /// </summary>
    public TestRow Row { get; set; } = null!;
}
