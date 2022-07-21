namespace Zomp.EFCore.BinaryFunctions.SqlServer.Infrastructure.Internal;

/// <summary>
/// Extensions for DbContextOptions.
/// </summary>
public class SqlServerDbContextOptionsExtension : IDbContextOptionsExtension
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

        private new SqlServerDbContextOptionsExtension Extension
            => (SqlServerDbContextOptionsExtension)base.Extension;
    }
}