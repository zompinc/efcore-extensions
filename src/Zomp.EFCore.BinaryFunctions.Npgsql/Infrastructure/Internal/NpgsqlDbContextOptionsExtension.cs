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

    private sealed class ExtensionInfo : BinaryFunctions.Infrastructure.Internal.ExtensionInfo
    {
        public ExtensionInfo(IDbContextOptionsExtension extension)
            : base(extension)
        {
        }

        private new NpgsqlDbContextOptionsExtension Extension
            => (NpgsqlDbContextOptionsExtension)base.Extension;
    }
}