namespace Zomp.EFCore.WindowFunctions.Sqlite;

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
        this SqliteDbContextOptionsBuilder builder)
    {
        return builder.AddOrUpdateExtension();
    }

    private static SqliteDbContextOptionsBuilder AddOrUpdateExtension(
        this SqliteDbContextOptionsBuilder sqliteOptionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(sqliteOptionsBuilder);

        var coreOptionsBuilder = ((IRelationalDbContextOptionsBuilderInfrastructure)sqliteOptionsBuilder).OptionsBuilder;
        var extension = coreOptionsBuilder.Options.FindExtension<SqliteDbContextOptionsExtension>() ?? new SqliteDbContextOptionsExtension();

        ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);
        coreOptionsBuilder.ReplaceService<IRelationalParameterBasedSqlProcessorFactory, WindowRelationalParameterBasedSqlProcessorFactory>();
        coreOptionsBuilder.ReplaceService<IQuerySqlGeneratorFactory, WindowQuerySqlGeneratorFactory>();
        coreOptionsBuilder.ReplaceService<IWindowFunctionsTranslatorPluginFactory, SqliteWindowFunctionsTranslatorPluginFactory>();
        coreOptionsBuilder.ReplaceService<IEvaluatableExpressionFilter, SqliteWindowFunctionsEvaluatableExpressionFilter>();

        return sqliteOptionsBuilder;
    }
}