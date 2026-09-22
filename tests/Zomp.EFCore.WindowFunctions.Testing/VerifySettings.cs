namespace Zomp.EFCore.WindowFunctions.Testing;

/// <summary>
/// Settings for the SQL snapshots.
/// </summary>
internal static class VerifySettings
{
    /// <summary>
    /// Keeps each provider's snapshots in its own test project. The tests are shared, so by default all three providers
    /// would compare against the same file next to the test source.
    /// </summary>
    [System.Runtime.CompilerServices.ModuleInitializer]
    internal static void Initialize() => UseProjectRelativeDirectory("Snapshots");
}
