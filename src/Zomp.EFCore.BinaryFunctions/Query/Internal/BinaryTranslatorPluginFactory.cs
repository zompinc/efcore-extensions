namespace Zomp.EFCore.BinaryFunctions.Query.Internal;

/// <summary>
/// Factory for BinaryTranslatorPlugin.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BinaryTranslatorPluginFactory"/> class.
/// </remarks>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
/// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
public class BinaryTranslatorPluginFactory(ISqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource relationalTypeMappingSource) : IBinaryTranslatorPluginFactory
{
    private readonly ISqlExpressionFactory sqlExpressionFactory = sqlExpressionFactory;
    private readonly IRelationalTypeMappingSource relationalTypeMappingSource = relationalTypeMappingSource;

    /// <inheritdoc/>
    public virtual BinaryTranslator Create() => new(sqlExpressionFactory, relationalTypeMappingSource);
}