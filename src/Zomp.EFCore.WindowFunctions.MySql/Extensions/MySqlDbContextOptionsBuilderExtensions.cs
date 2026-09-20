#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Zomp.EFCore.WindowFunctions.MySql;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Window function extension methods for <see cref="MySqlDbContextOptionsBuilder" />.
/// </summary>
public static class MySqlDbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Use window functions.
    /// </summary>
    /// <param name="builder">The build being used to configure MySQL.</param>
    /// <returns>The same builder so that further configuration can be chained.</returns>
    public static MySqlDbContextOptionsBuilder UseWindowFunctions(this MySqlDbContextOptionsBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var coreOptionsBuilder = ((IRelationalDbContextOptionsBuilderInfrastructure)builder).OptionsBuilder;
        var extension = coreOptionsBuilder.Options.FindExtension<MySqlWindowFunctionsOptionsExtension>() ?? new MySqlWindowFunctionsOptionsExtension();

        ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);
        _ = coreOptionsBuilder.ReplaceService<IRelationalParameterBasedSqlProcessorFactory, WindowFunctionsMySqlParameterBasedSqlProcessorFactory>();
        _ = coreOptionsBuilder.ReplaceService<IQuerySqlGeneratorFactory, WindowFunctionsMySqlQuerySqlGeneratorFactory>();
        _ = coreOptionsBuilder.ReplaceService<IWindowFunctionsTranslatorPluginFactory, MySqlWindowFunctionsTranslatorPluginFactory>();
        _ = coreOptionsBuilder.ReplaceService<IEvaluatableExpressionFilter, WindowFunctionsMySqlEvaluatableExpressionFilter>();
        _ = coreOptionsBuilder.ReplaceService<IQueryableMethodTranslatingExpressionVisitorFactory, WindowFunctionsMySqlQueryableMethodTranslatingExpressionVisitorFactory>();
        _ = coreOptionsBuilder.ReplaceService<IQueryTranslationPreprocessorFactory, WindowFunctionsRelationalQueryTranslationPreprocessorFactory>();

        return builder;
    }
}