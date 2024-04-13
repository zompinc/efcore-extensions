namespace Zomp.EFCore.BinaryFunctions.Infrastructure.Internal;

/// <summary>
/// Information/metadata for the extension.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ExtensionInfo"/> class.
/// </remarks>
/// <param name="extension">The extension.</param>
public class ExtensionInfo(IDbContextOptionsExtension extension) : DbContextOptionsExtensionInfo(extension)
{
    /// <inheritdoc/>
    public override bool IsDatabaseProvider
        => false;

    /// <inheritdoc/>
    public override string LogFragment
        => "using Binary Function support ";

    /// <inheritdoc/>
    public override int GetServiceProviderHashCode()
        => 0;

    /// <inheritdoc/>
    public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
        => other is ExtensionInfo;

    /// <inheritdoc/>
    public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        => debugInfo["Binary Functions support:"] = "1";
}