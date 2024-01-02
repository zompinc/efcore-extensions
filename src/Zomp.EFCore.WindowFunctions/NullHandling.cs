namespace Zomp.EFCore.WindowFunctions;

/// <summary>
/// Specifies whether null should be respected or ignored.
/// </summary>
public enum NullHandling
{
    /// <summary>
    /// Respect nulls.
    /// </summary>
    RespectNulls,

    /// <summary>
    /// Ignore nulls.
    /// </summary>
    IgnoreNulls,
}