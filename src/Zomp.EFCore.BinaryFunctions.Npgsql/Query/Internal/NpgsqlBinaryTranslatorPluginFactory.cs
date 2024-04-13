namespace Zomp.EFCore.BinaryFunctions.Npgsql.Query.Internal;

/// <summary>
/// Binary translator plugin factory for Postgres provider.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NpgsqlBinaryTranslatorPluginFactory"/> class.
/// </remarks>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
/// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
public class NpgsqlBinaryTranslatorPluginFactory(ISqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource relationalTypeMappingSource) : BinaryTranslatorPluginFactory(sqlExpressionFactory, relationalTypeMappingSource)
{
    private readonly ISqlExpressionFactory sqlExpressionFactory = sqlExpressionFactory;
    private readonly IRelationalTypeMappingSource relationalTypeMappingSource = relationalTypeMappingSource;

    /// <inheritdoc/>
    public override BinaryTranslator Create()
        => new NpgsqlBinaryTranslator(sqlExpressionFactory, relationalTypeMappingSource);
}