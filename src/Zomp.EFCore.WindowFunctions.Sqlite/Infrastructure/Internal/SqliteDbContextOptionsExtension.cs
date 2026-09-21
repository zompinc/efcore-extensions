namespace Zomp.EFCore.WindowFunctions.Sqlite.Infrastructure.Internal;

/// <summary>
/// Extensions for DbContextOptions.
/// </summary>
public class SqliteDbContextOptionsExtension : IDbContextOptionsExtension
{
    private ExtensionInfo? info;

    /// <summary>
    /// Gets a value indicating whether standard deviation and variance are computed from AVG, SUM and COUNT,
    /// which SQLite has, rather than throwing.
    /// </summary>
    public bool ApproximateStandardDeviationAndVariance { get; init; }

    /// <inheritdoc/>
    public DbContextOptionsExtensionInfo Info => info ??= new(this);

    /// <inheritdoc/>
    public void ApplyServices(IServiceCollection services) => services.AddWindowedFunctionsExtension();

    /// <inheritdoc/>
    public void Validate(IDbContextOptions options)
    {
    }
}