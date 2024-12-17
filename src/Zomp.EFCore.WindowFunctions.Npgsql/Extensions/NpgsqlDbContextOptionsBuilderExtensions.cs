#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Zomp.EFCore.WindowFunctions.Npgsql;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Window function extension methods for <see cref="NpgsqlDbContextOptionsBuilder" />.
/// </summary>
public static class NpgsqlDbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Use window functions.
    /// </summary>
    /// <param name="builder">The build being used to configure Postgres.</param>
    /// <returns>The same builder so that further configuration can be chained.</returns>
    public static NpgsqlDbContextOptionsBuilder UseWindowFunctions(
        this NpgsqlDbContextOptionsBuilder builder) => builder.AddOrUpdateExtension();

    private static NpgsqlDbContextOptionsBuilder AddOrUpdateExtension(
        this NpgsqlDbContextOptionsBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var coreOptionsBuilder = ((IRelationalDbContextOptionsBuilderInfrastructure)builder).OptionsBuilder;
        var extension = coreOptionsBuilder.Options.FindExtension<NpgsqlDbContextOptionsExtension>() ?? new NpgsqlDbContextOptionsExtension();

        ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);
        _ = coreOptionsBuilder.ReplaceService<IRelationalParameterBasedSqlProcessorFactory, WindowFunctionsNpgsqlParameterBasedSqlProcessorFactory>();
        _ = coreOptionsBuilder.ReplaceService<IQuerySqlGeneratorFactory, WindowFunctionsNpgsqlQuerySqlGeneratorFactory>();
        _ = coreOptionsBuilder.ReplaceService<IEvaluatableExpressionFilter, WindowFunctionsNpgsqlEvaluatableExpressionFilter>();
        _ = coreOptionsBuilder.ReplaceService<IQueryableMethodTranslatingExpressionVisitorFactory, WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitorFactory>();
        _ = coreOptionsBuilder.ReplaceService<IQueryTranslationPreprocessorFactory, WindowFunctionsRelationalQueryTranslationPreprocessorFactory>();

        return builder;
    }
}