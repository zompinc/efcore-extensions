namespace Zomp.EFCore.WindowFunctions.MySql.Infrastructure.Internal;

/// <summary>
/// MySQL options extension which registers the window function services.
/// </summary>
public class MySqlWindowFunctionsOptionsExtension : IDbContextOptionsExtension
{
    private ExtensionInfo? info;

    /// <inheritdoc/>
    public DbContextOptionsExtensionInfo Info => info ??= new(this);

    /// <inheritdoc/>
    public void ApplyServices(IServiceCollection services) => services.AddWindowedFunctionsExtension();

    /// <inheritdoc/>
    public void Validate(IDbContextOptions options)
    {
    }
}