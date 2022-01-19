namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Factory for BinaryTranslatorPlugin.
/// </summary>
public class BinaryTranslatorPluginFactory : IBinaryTranslatorPluginFactory
{
    private readonly ISqlExpressionFactory sqlExpressionFactory;
    private readonly IRelationalTypeMappingSource relationalTypeMappingSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryTranslatorPluginFactory"/> class.
    /// </summary>
    /// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
    /// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
    public BinaryTranslatorPluginFactory(ISqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource relationalTypeMappingSource)
    {
        this.sqlExpressionFactory = sqlExpressionFactory;
        this.relationalTypeMappingSource = relationalTypeMappingSource;
    }

    /// <inheritdoc/>
    public virtual BinaryTranslator Create()
    {
        return new BinaryTranslator(sqlExpressionFactory, relationalTypeMappingSource);
    }
}