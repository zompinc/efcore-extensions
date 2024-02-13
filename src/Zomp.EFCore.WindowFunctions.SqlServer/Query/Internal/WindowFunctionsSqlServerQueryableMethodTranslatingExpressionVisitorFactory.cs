namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// WindowFunctionsSqlServerQueryableMethodTranslatingExpressionVisitorFactory.
/// </summary>
/// <param name="dependencies">Type mapping source dependencies.</param>
/// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
/// <param name="sqlServerSingletonOptions">The singleton option.</param>
public class WindowFunctionsSqlServerQueryableMethodTranslatingExpressionVisitorFactory(QueryableMethodTranslatingExpressionVisitorDependencies dependencies, RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies, ISqlServerSingletonOptions sqlServerSingletonOptions)
    : SqlServerQueryableMethodTranslatingExpressionVisitorFactory(dependencies, relationalDependencies, sqlServerSingletonOptions)
{
    private readonly QueryableMethodTranslatingExpressionVisitorDependencies dependencies = dependencies;
    private readonly RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies = relationalDependencies;
    private readonly ISqlServerSingletonOptions sqlServerSingletonOptions = sqlServerSingletonOptions;

    /// <inheritdoc/>
    public override QueryableMethodTranslatingExpressionVisitor Create(QueryCompilationContext queryCompilationContext)
    {
        return new WindowFunctionsSqlServerQueryableMethodTranslatingExpressionVisitor(
            dependencies, relationalDependencies, (SqlServerQueryCompilationContext)queryCompilationContext, sqlServerSingletonOptions);
    }
}