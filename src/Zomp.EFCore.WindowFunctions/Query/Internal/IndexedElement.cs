namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// A row and its index, which Where with an index is filtered on.
/// </summary>
/// <typeparam name="T">The type of the row.</typeparam>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Created in the expression tree of the rewritten query")]
internal sealed class IndexedElement<T>
{
    /// <summary>
    /// Gets or sets the row.
    /// </summary>
    public T Item { get; set; } = default!;

    /// <summary>
    /// Gets or sets the index of the row.
    /// </summary>
    public int Index { get; set; }
}
