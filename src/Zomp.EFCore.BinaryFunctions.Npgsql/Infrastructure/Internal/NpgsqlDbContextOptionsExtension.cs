namespace Zomp.EFCore.BinaryFunctions.Npgsql.Infrastructure.Internal;

/// <summary>
/// Extensions for DbContextOptions.
/// </summary>
public class NpgsqlDbContextOptionsExtension : IDbContextOptionsExtension
{
    private ExtensionInfo? info;

    /// <inheritdoc/>
    public DbContextOptionsExtensionInfo Info => info ??= new ExtensionInfo(this);

    /// <inheritdoc/>
    public void ApplyServices(IServiceCollection services) => services.AddBinaryFunctionsExtension();

    /// <inheritdoc/>
    public void Validate(IDbContextOptions options)
    {
    }

    private sealed class ExtensionInfo(IDbContextOptionsExtension extension) : BinaryFunctions.Infrastructure.Internal.ExtensionInfo(extension)
    {
        public override IDbContextOptionsExtension Extension
            => (NpgsqlDbContextOptionsExtension)base.Extension;
    }
}