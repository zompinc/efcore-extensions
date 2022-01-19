namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Window frame.
/// </summary>
/// <remarks>
/// Values could be UNBOUNDED, N, CURRENT ROW.
/// </remarks>
public abstract record WindowFrame
{
    /// <summary>
    /// Gets a value indicating whether window frame can be preceding or following.
    /// </summary>
    public abstract bool IsDirectional { get; }

    internal static UnboundedWindowFrame Unbounded { get; } = new UnboundedWindowFrame();

    internal static CurrentRowWindowFrame CurrentRow { get; } = new CurrentRowWindowFrame();
}