namespace Zomp.EFCore.BinaryFunctions.Sqlite.Query.Internal;

/// <summary>
/// Factory for <see cref="BinaryTranslator"/> instances.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SqliteBinaryTranslatorPluginFactory"/> class.
/// </remarks>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
/// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
public class SqliteBinaryTranslatorPluginFactory(ISqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource relationalTypeMappingSource) : BinaryTranslatorPluginFactory(sqlExpressionFactory, relationalTypeMappingSource)
{
    private readonly ISqlExpressionFactory sqlExpressionFactory = sqlExpressionFactory;
    private readonly IRelationalTypeMappingSource relationalTypeMappingSource = relationalTypeMappingSource;

    /// <inheritdoc/>
    public override BinaryTranslator Create()
        => new SqliteBinaryTranslator(sqlExpressionFactory, relationalTypeMappingSource);
}