namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// Window functions translator plugin factory for the PostgreSQL provider.
/// </summary>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
public class NpgsqlWindowFunctionsTranslatorPluginFactory(ISqlExpressionFactory sqlExpressionFactory)
    : WindowFunctionsTranslatorPluginFactory(sqlExpressionFactory)
{
    private readonly ISqlExpressionFactory sqlExpressionFactory = sqlExpressionFactory;

    /// <inheritdoc/>
    public override WindowFunctionsTranslator Create()
        => new NpgsqlWindowFunctionsTranslator(sqlExpressionFactory);
}