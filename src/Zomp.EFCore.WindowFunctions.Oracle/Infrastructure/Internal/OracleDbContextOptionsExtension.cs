namespace Zomp.EFCore.WindowFunctions.Oracle.Infrastructure.Internal;

/// <summary>
/// Extensions for DbContextOptions.
/// </summary>
public class OracleDbContextOptionsExtension : IDbContextOptionsExtension
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
            => (OracleDbContextOptionsExtension)base.Extension;
    }
}