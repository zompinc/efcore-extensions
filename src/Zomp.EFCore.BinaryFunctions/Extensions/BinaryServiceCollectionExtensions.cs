namespace Zomp.EFCore.BinaryFunctions;

/// <summary>
/// Binary function extension methods for <see cref="IServiceCollection" />.
/// </summary>
public static class BinaryServiceCollectionExtensions
{
    /// <summary>
    /// Adds the services required to run binary functions.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddBinaryFunctionsExtension(
        this IServiceCollection serviceCollection)
    {
        new EntityFrameworkRelationalServicesBuilder(serviceCollection)
            .TryAdd<IMethodCallTranslatorPlugin, BinaryFunctionsTranslatorPlugin>()
            .TryAddProviderSpecificServices(b => b
                .TryAddScoped<IBinaryTranslatorPluginFactory, BinaryTranslatorPluginFactory>());

        return serviceCollection;
    }
}