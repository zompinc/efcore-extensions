namespace Zomp.EFCore.BinaryFunctions.Sqlite.Query.Internal;

/// <summary>
/// Factory for <see cref="BinaryTranslator"/> instances.
/// </summary>
public class SqliteBinaryTranslatorPluginFactory : BinaryTranslatorPluginFactory
{
    private readonly ISqlExpressionFactory sqlExpressionFactory;
    private readonly IRelationalTypeMappingSource relationalTypeMappingSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteBinaryTranslatorPluginFactory"/> class.
    /// </summary>
    /// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
    /// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
    public SqliteBinaryTranslatorPluginFactory(ISqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource relationalTypeMappingSource)
        : base(sqlExpressionFactory, relationalTypeMappingSource)
    {
        this.sqlExpressionFactory = sqlExpressionFactory;
        this.relationalTypeMappingSource = relationalTypeMappingSource;
    }

    /// <inheritdoc/>
    public override BinaryTranslator Create()
        => new SqliteBinaryTranslator(sqlExpressionFactory, relationalTypeMappingSource);
}