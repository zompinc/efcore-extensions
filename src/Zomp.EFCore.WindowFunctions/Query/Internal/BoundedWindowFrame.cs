namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Represents bounded window frame.
/// </summary>
/// <param name="Value">Gets a value indicating the row count.</param>
/// <param name="IsFollowing">Gets a value indicating whether the value is following.</param>
public record BoundedWindowFrame(uint Value, bool IsFollowing) : WindowFrame
{
    /// <inheritdoc/>
    public override bool IsDirectional => true;

    /// <inheritdoc/>
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}