namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Factory for WindowFunctionsTranslatorPlugin.
/// </summary>
public class WindowFunctionsTranslatorPluginFactory : IWindowFunctionsTranslatorPluginFactory
{
    private readonly ISqlExpressionFactory sqlExpressionFactory;
    private readonly IRelationalTypeMappingSource relationalTypeMappingSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsTranslatorPluginFactory"/> class.
    /// </summary>
    /// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
    /// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
    public WindowFunctionsTranslatorPluginFactory(ISqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource relationalTypeMappingSource)
    {
        this.sqlExpressionFactory = sqlExpressionFactory;
        this.relationalTypeMappingSource = relationalTypeMappingSource;
    }

    /// <inheritdoc/>
    public virtual WindowFunctionsTranslator Create()
    {
        return new WindowFunctionsTranslator(sqlExpressionFactory, relationalTypeMappingSource);
    }
}