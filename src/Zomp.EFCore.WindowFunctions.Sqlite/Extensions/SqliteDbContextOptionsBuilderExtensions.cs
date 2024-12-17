#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Zomp.EFCore.WindowFunctions.Sqlite;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Window function extension methods for <see cref="SqliteDbContextOptionsBuilder" />.
/// </summary>
public static class SqliteDbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Use window functions.
    /// </summary>
    /// <param name="builder">The build being used to configure Postgres.</param>
    /// <returns>The same builder so that further configuration can be chained.</returns>
    public static SqliteDbContextOptionsBuilder UseWindowFunctions(
        this SqliteDbContextOptionsBuilder builder) => builder.AddOrUpdateExtension();

    private static SqliteDbContextOptionsBuilder AddOrUpdateExtension(
        this SqliteDbContextOptionsBuilder sqliteOptionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(sqliteOptionsBuilder);

        var coreOptionsBuilder = ((IRelationalDbContextOptionsBuilderInfrastructure)sqliteOptionsBuilder).OptionsBuilder;
        var extension = coreOptionsBuilder.Options.FindExtension<SqliteDbContextOptionsExtension>() ?? new SqliteDbContextOptionsExtension();

        ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);
        _ = coreOptionsBuilder.ReplaceService<
            IRelationalParameterBasedSqlProcessorFactory,
            WindowFunctionsSqliteParameterBasedSqlProcessorFactory
        >()
        .ReplaceService<IQuerySqlGeneratorFactory, WindowQuerySqlGeneratorFactory>()
        .ReplaceService<IWindowFunctionsTranslatorPluginFactory, SqliteWindowFunctionsTranslatorPluginFactory>()
        .ReplaceService<IEvaluatableExpressionFilter, SqliteWindowFunctionsEvaluatableExpressionFilter>()
        .ReplaceService<IQueryableMethodTranslatingExpressionVisitorFactory, WindowFunctionsSqliteQueryableMethodTranslatingExpressionVisitorFactory>()
        .ReplaceService<IQueryTranslationPreprocessorFactory, WindowFunctionsRelationalQueryTranslationPreprocessorFactory>();

        return sqliteOptionsBuilder;
    }
}