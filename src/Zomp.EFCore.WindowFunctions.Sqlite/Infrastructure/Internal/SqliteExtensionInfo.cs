namespace Zomp.EFCore.WindowFunctions.Sqlite.Infrastructure.Internal;

/// <summary>
/// Information about the SQLite window functions extension, including its options.
/// </summary>
/// <remarks>
/// A compiled query is cached per internal service provider, so contexts that translate differently must not share one.
/// </remarks>
/// <param name="extension">The extension.</param>
public class SqliteExtensionInfo(SqliteDbContextOptionsExtension extension) : ExtensionInfo(extension)
{
    private readonly SqliteDbContextOptionsExtension extension = extension;

    /// <inheritdoc/>
    public override int GetServiceProviderHashCode()
        => extension.ApproximateStandardDeviationAndVariance.GetHashCode();

    /// <inheritdoc/>
    public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
        => other is SqliteExtensionInfo { extension.ApproximateStandardDeviationAndVariance: var approximate }
            && approximate == extension.ApproximateStandardDeviationAndVariance;

    /// <inheritdoc/>
    public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
    {
        ArgumentNullException.ThrowIfNull(debugInfo);
        base.PopulateDebugInfo(debugInfo);
        debugInfo["Window Functions:ApproximateStandardDeviationAndVariance"] = extension.ApproximateStandardDeviationAndVariance.ToString();
    }
}