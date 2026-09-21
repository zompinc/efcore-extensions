namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// Window functions translator plugin factory for the SQL Server provider.
/// </summary>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
public class SqlServerWindowFunctionsTranslatorPluginFactory(ISqlExpressionFactory sqlExpressionFactory)
    : WindowFunctionsTranslatorPluginFactory(sqlExpressionFactory)
{
    private readonly ISqlExpressionFactory sqlExpressionFactory = sqlExpressionFactory;

    /// <inheritdoc/>
    public override WindowFunctionsTranslator Create()
        => new SqlServerWindowFunctionsTranslator(sqlExpressionFactory);
}