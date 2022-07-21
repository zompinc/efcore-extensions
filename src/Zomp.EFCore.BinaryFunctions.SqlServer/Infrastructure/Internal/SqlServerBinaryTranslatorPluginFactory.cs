namespace Zomp.EFCore.BinaryFunctions.SqlServer.Infrastructure.Internal;

/// <summary>
/// Sql Server BinaryTranslator Plugin Factory.
/// </summary>
public class SqlServerBinaryTranslatorPluginFactory : BinaryTranslatorPluginFactory
{
    private readonly ISqlExpressionFactory sqlExpressionFactory;
    private readonly IRelationalTypeMappingSource relationalTypeMappingSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerBinaryTranslatorPluginFactory"/> class.
    /// </summary>
    /// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
    /// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
    public SqlServerBinaryTranslatorPluginFactory(ISqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource relationalTypeMappingSource)
        : base(sqlExpressionFactory, relationalTypeMappingSource)
    {
        this.sqlExpressionFactory = sqlExpressionFactory;
        this.relationalTypeMappingSource = relationalTypeMappingSource;
    }

    /// <inheritdoc/>
    public override BinaryTranslator Create()
        => new SqlServerBinaryTranslator(sqlExpressionFactory, relationalTypeMappingSource);
}