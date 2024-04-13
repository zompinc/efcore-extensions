namespace Zomp.EFCore.BinaryFunctions.SqlServer.Infrastructure.Internal;

/// <summary>
/// Sql Server BinaryTranslator Plugin Factory.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SqlServerBinaryTranslatorPluginFactory"/> class.
/// </remarks>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
/// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
public class SqlServerBinaryTranslatorPluginFactory(ISqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource relationalTypeMappingSource) : BinaryTranslatorPluginFactory(sqlExpressionFactory, relationalTypeMappingSource)
{
    private readonly ISqlExpressionFactory sqlExpressionFactory = sqlExpressionFactory;
    private readonly IRelationalTypeMappingSource relationalTypeMappingSource = relationalTypeMappingSource;

    /// <inheritdoc/>
    public override BinaryTranslator Create()
        => new SqlServerBinaryTranslator(sqlExpressionFactory, relationalTypeMappingSource);
}