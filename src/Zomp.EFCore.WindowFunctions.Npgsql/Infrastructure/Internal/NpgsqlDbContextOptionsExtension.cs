namespace Zomp.EFCore.WindowFunctions.Npgsql.Infrastructure.Internal;

/// <summary>
/// Extensions for DbContextOptions.
/// </summary>
public class NpgsqlDbContextOptionsExtension : IDbContextOptionsExtension
{
    private ExtensionInfo? info;

    /// <inheritdoc/>
    public DbContextOptionsExtensionInfo Info => info ??= new ExtensionInfo(this);

    /// <inheritdoc/>
    public void ApplyServices(IServiceCollection services) => services.AddWindowedFunctionsExtension();

    /// <inheritdoc/>
    public void Validate(IDbContextOptions options)
    {
    }

    private sealed class ExtensionInfo(IDbContextOptionsExtension extension) : WindowFunctions.Infrastructure.Internal.ExtensionInfo(extension)
    {
        public override IDbContextOptionsExtension Extension
            => (NpgsqlDbContextOptionsExtension)base.Extension;
    }
}