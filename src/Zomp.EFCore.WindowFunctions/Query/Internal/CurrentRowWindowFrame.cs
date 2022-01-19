namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Current row window frame.
/// </summary>
public record CurrentRowWindowFrame : WindowFrame
{
    /// <inheritdoc/>
    public override bool IsDirectional => false;

    /// <inheritdoc/>
    public override string ToString() => "CURRENT ROW";
}