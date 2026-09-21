namespace Zomp.EFCore.WindowFunctions.Sqlite.Query.Internal;

/// <summary>
/// Window functions translator plugin factory for SQLite provider.
/// </summary>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
/// <param name="options">The options of the context, which say whether to approximate standard deviation and variance.</param>
public class SqliteWindowFunctionsTranslatorPluginFactory(ISqlExpressionFactory sqlExpressionFactory, IDbContextOptions options)
    : WindowFunctionsTranslatorPluginFactory(sqlExpressionFactory)
{
    private readonly ISqlExpressionFactory sqlExpressionFactory = sqlExpressionFactory;

    /// <inheritdoc/>
    public override WindowFunctionsTranslator Create()
        => new SqliteWindowFunctionsTranslator(
            sqlExpressionFactory,
            options.FindExtension<SqliteDbContextOptionsExtension>()?.ApproximateStandardDeviationAndVariance ?? false);
}