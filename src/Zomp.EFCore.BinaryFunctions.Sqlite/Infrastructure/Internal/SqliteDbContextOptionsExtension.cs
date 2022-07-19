namespace Zomp.EFCore.BinaryFunctions.Sqlite.Infrastructure.Internal;

/// <summary>
/// Extensions for DbContextOptions.
/// </summary>
public class SqliteDbContextOptionsExtension : IDbContextOptionsExtension
{
    private ExtensionInfo? info;

    /// <inheritdoc/>
    public DbContextOptionsExtensionInfo Info => info ??= new(this);

    /// <inheritdoc/>
    public void ApplyServices(IServiceCollection services) => services.AddBinaryFunctionsExtension();

    /// <inheritdoc/>
    public void Validate(IDbContextOptions options)
    {
    }
}