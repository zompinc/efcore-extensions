namespace Zomp.EFCore.WindowFunctions.Generators.Tests;

/// <summary>
/// Settings for the generator snapshots.
/// </summary>
internal static class VerifySettings
{
    /// <summary>
    /// Keeps the snapshots in a Snapshots folder, as the database test projects do.
    /// </summary>
    [System.Runtime.CompilerServices.ModuleInitializer]
    internal static void Initialize() => UseProjectRelativeDirectory("Snapshots");
}
