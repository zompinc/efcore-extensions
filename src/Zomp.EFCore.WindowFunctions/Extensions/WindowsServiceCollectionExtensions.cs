﻿#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Zomp.EFCore.WindowFunctions;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Window function extension methods for <see cref="IServiceCollection" />.
/// </summary>
public static class WindowsServiceCollectionExtensions
{
    /// <summary>
    /// Adds the services required to run window functions.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddWindowedFunctionsExtension(
        this IServiceCollection serviceCollection)
    {
        _ = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
            .TryAdd<IMethodCallTranslatorPlugin, WindowFunctionsTranslatorPlugin>()
            .TryAddProviderSpecificServices(b => b
                .TryAddScoped<IWindowFunctionsTranslatorPluginFactory, WindowFunctionsTranslatorPluginFactory>());

        return serviceCollection;
    }
}