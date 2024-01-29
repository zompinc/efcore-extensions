namespace Zomp.EFCore.WindowFunctions.SqlServer;

/// <summary>
/// Window function extension methods for <see cref="SqlServerDbContextOptionsBuilder" />.
/// </summary>
public static class SqlServerDbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Use window functions.
    /// </summary>
    /// <param name="builder">The build being used to configure Postgres.</param>
    /// <returns>The same builder so that further configuration can be chained.</returns>
    public static SqlServerDbContextOptionsBuilder UseWindowFunctions(
       this SqlServerDbContextOptionsBuilder builder)
    {
        builder.AddOrUpdateExtension();
        return builder;
    }

    private static SqlServerDbContextOptionsBuilder AddOrUpdateExtension(
        this SqlServerDbContextOptionsBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var coreOptionsBuilder = ((IRelationalDbContextOptionsBuilderInfrastructure)builder).OptionsBuilder;
        var extension = coreOptionsBuilder.Options.FindExtension<SqlServerDbContextOptionsExtension>() ?? new SqlServerDbContextOptionsExtension();

        ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);
        coreOptionsBuilder.ReplaceService<IRelationalParameterBasedSqlProcessorFactory, WindowFunctionsSqlServerParameterBasedSqlProcessorFactory>();
        coreOptionsBuilder.ReplaceService<IQuerySqlGeneratorFactory, WindowFunctionsSqlServerQuerySqlGeneratorFactory>();
        coreOptionsBuilder.ReplaceService<IQueryableMethodTranslatingExpressionVisitorFactory, WindowFunctionsSqlServerQueryableMethodTranslatingExpressionVisitorFactory>();
        coreOptionsBuilder.ReplaceService<IQueryTranslationPreprocessorFactory, WindowFunctionsRelationalQueryTranslationPreprocessorFactory>();

        return builder;
    }
}