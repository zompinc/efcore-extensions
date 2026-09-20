namespace Zomp.EFCore.WindowFunctions.MySql.Query.Internal;

/// <summary>
/// Window functions translator plugin factory for the MySQL provider.
/// </summary>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
public class MySqlWindowFunctionsTranslatorPluginFactory(ISqlExpressionFactory sqlExpressionFactory)
    : WindowFunctionsTranslatorPluginFactory(sqlExpressionFactory)
{
    private readonly ISqlExpressionFactory sqlExpressionFactory = sqlExpressionFactory;

    /// <inheritdoc/>
    public override WindowFunctionsTranslator Create()
        => new MySqlWindowFunctionsTranslator(sqlExpressionFactory);
}