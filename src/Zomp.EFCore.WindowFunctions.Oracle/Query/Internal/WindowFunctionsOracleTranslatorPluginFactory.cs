namespace Zomp.EFCore.WindowFunctions.Oracle.Query.Internal;

/// <summary>
/// Window functions translator plugin factory for SQLite provider.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsOracleTranslatorPluginFactory"/> class.
/// </remarks>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
public class WindowFunctionsOracleTranslatorPluginFactory(ISqlExpressionFactory sqlExpressionFactory) : WindowFunctionsTranslatorPluginFactory(sqlExpressionFactory)
{
    private readonly ISqlExpressionFactory sqlExpressionFactory = sqlExpressionFactory;

    /// <inheritdoc/>
    public override WindowFunctionsTranslator Create()
        => new WindowFunctionsOracleTranslator(sqlExpressionFactory);
}