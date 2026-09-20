#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Zomp.EFCore.WindowFunctions.Oracle;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Window function extension methods for <see cref="OracleDbContextOptionsBuilder" />.
/// </summary>
public static class OracleDbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Use window functions.
    /// </summary>
    /// <param name="builder">The build being used to configure Postgres.</param>
    /// <returns>The same builder so that further configuration can be chained.</returns>
    public static OracleDbContextOptionsBuilder UseWindowFunctions(
       this OracleDbContextOptionsBuilder builder)
    {
        _ = builder.AddOrUpdateExtension();
        return builder;
    }

    private static OracleDbContextOptionsBuilder AddOrUpdateExtension(
        this OracleDbContextOptionsBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var coreOptionsBuilder = ((IRelationalDbContextOptionsBuilderInfrastructure)builder).OptionsBuilder;
        var extension = coreOptionsBuilder.Options.FindExtension<OracleDbContextOptionsExtension>() ?? new OracleDbContextOptionsExtension();

        ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);
        _ = coreOptionsBuilder.ReplaceService<IRelationalParameterBasedSqlProcessorFactory, WindowFunctionsOracleParameterBasedSqlProcessorFactory>();
        _ = coreOptionsBuilder.ReplaceService<IQuerySqlGeneratorFactory, WindowFunctionsOracleQuerySqlGeneratorFactory>();
        _ = coreOptionsBuilder.ReplaceService<IWindowFunctionsTranslatorPluginFactory, WindowFunctionsOracleTranslatorPluginFactory>();
        _ = coreOptionsBuilder.ReplaceService<IQueryableMethodTranslatingExpressionVisitorFactory, WindowFunctionsOracleQueryableMethodTranslatingExpressionVisitorFactory>();
        _ = coreOptionsBuilder.ReplaceService<IQueryTranslationPreprocessorFactory, WindowFunctionsRelationalQueryTranslationPreprocessorFactory>();

        return builder;
    }
}