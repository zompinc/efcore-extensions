namespace Zomp.EFCore.WindowFunctions.Testing;

/// <summary>
/// Settings for the SQL snapshots.
/// </summary>
internal static class SnapshotSettings
{
    /// <summary>
    /// Keeps each provider's snapshots in its own test project. The tests are shared, so by default all three providers
    /// would compare against the same file next to the test source.
    /// </summary>
    [System.Runtime.CompilerServices.ModuleInitializer]
    internal static void Initialize() => UseProjectRelativeDirectory("Snapshots");

    /// <summary>
    /// Verifies SQL that EF Core generates differently from one major version to the next (fewer casts, ISNULL for
    /// COALESCE, simpler null checks) against a snapshot per target framework, since each targets another EF Core.
    /// </summary>
    /// <param name="sql">The SQL to verify.</param>
    /// <returns>The verification task.</returns>
    internal static VerifyTests.SettingsTask VerifyPerFramework(string sql) => Verify(sql).UniqueForTargetFrameworkAndVersion();
}
