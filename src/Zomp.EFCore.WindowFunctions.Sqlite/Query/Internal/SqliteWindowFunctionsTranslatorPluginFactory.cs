namespace Zomp.EFCore.WindowFunctions.Sqlite.Query.Internal;

/// <summary>
/// Window functions translator plugin factory for SQLite provider.
/// </summary>
public class SqliteWindowFunctionsTranslatorPluginFactory : WindowFunctionsTranslatorPluginFactory
{
    private readonly ISqlExpressionFactory sqlExpressionFactory;
    private readonly IRelationalTypeMappingSource relationalTypeMappingSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteWindowFunctionsTranslatorPluginFactory"/> class.
    /// </summary>
    /// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
    /// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
    public SqliteWindowFunctionsTranslatorPluginFactory(ISqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource relationalTypeMappingSource)
        : base(sqlExpressionFactory, relationalTypeMappingSource)
    {
        this.sqlExpressionFactory = sqlExpressionFactory;
        this.relationalTypeMappingSource = relationalTypeMappingSource;
    }

    /// <inheritdoc/>
    public override WindowFunctionsTranslator Create()
        => new SqliteWindowFunctionsTranslator(sqlExpressionFactory, relationalTypeMappingSource);
}