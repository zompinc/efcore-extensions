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
        this SqliteDbContextOptionsBuilder builder) => builder.AddOrUpdateExtension(approximateStandardDeviationAndVariance: false);

    /// <summary>
    /// Use window functions.
    /// </summary>
    /// <param name="builder">The build being used to configure SQLite.</param>
    /// <param name="approximateStandardDeviationAndVariance">
    /// SQLite has no standard deviation or variance, so by default using one throws. When <see langword="true"/>, they are
    /// computed from AVG, SUM and COUNT over the same window instead. That loses precision when the values are large and
    /// close together, such as timestamps, and needs SQRT, which the SQLite bundled with Microsoft.Data.Sqlite has.
    /// </param>
    /// <returns>The same builder so that further configuration can be chained.</returns>
    public static SqliteDbContextOptionsBuilder UseWindowFunctions(
        this SqliteDbContextOptionsBuilder builder,
        bool approximateStandardDeviationAndVariance) => builder.AddOrUpdateExtension(approximateStandardDeviationAndVariance);

    private static SqliteDbContextOptionsBuilder AddOrUpdateExtension(
        this SqliteDbContextOptionsBuilder sqliteOptionsBuilder,
        bool approximateStandardDeviationAndVariance)
    {
        ArgumentNullException.ThrowIfNull(sqliteOptionsBuilder);

        var coreOptionsBuilder = ((IRelationalDbContextOptionsBuilderInfrastructure)sqliteOptionsBuilder).OptionsBuilder;
        var extension = new SqliteDbContextOptionsExtension { ApproximateStandardDeviationAndVariance = approximateStandardDeviationAndVariance };

        ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);
        _ = coreOptionsBuilder.ReplaceService<
            IRelationalParameterBasedSqlProcessorFactory,
            WindowFunctionsSqliteParameterBasedSqlProcessorFactory
        >()
        .ReplaceService<IQuerySqlGeneratorFactory, WindowFunctionsSqliteQuerySqlGeneratorFactory>()
        .ReplaceService<IWindowFunctionsTranslatorPluginFactory, SqliteWindowFunctionsTranslatorPluginFactory>()
        .ReplaceService<IEvaluatableExpressionFilter, SqliteWindowFunctionsEvaluatableExpressionFilter>()
        .ReplaceService<IQueryableMethodTranslatingExpressionVisitorFactory, WindowFunctionsSqliteQueryableMethodTranslatingExpressionVisitorFactory>()
        .ReplaceService<IQueryTranslationPreprocessorFactory, WindowFunctionsRelationalQueryTranslationPreprocessorFactory>();

        return sqliteOptionsBuilder;
    }
}