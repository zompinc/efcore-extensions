namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Unbounded window frame.
/// </summary>
public record UnboundedWindowFrame : WindowFrame
{
    /// <inheritdoc/>
    public override bool IsDirectional => true;

    /// <inheritdoc/>
    public override string ToString() => "UNBOUNDED";
}