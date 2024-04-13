namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// The WindowFunctionsSqlServerQueryableMethodTranslatingExpressionVisitor.
/// </summary>
/// <param name="dependencies">Type mapping source dependencies.</param>
/// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
/// <param name="queryCompilationContext">The query compilation context object to use.</param>
/// <param name="sqlServerSingletonOptions">The singleton option.</param>
public class WindowFunctionsSqlServerQueryableMethodTranslatingExpressionVisitor(QueryableMethodTranslatingExpressionVisitorDependencies dependencies, RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies, SqlServerQueryCompilationContext queryCompilationContext, ISqlServerSingletonOptions sqlServerSingletonOptions)
    : SqlServerQueryableMethodTranslatingExpressionVisitor(dependencies, relationalDependencies, queryCompilationContext, sqlServerSingletonOptions)
{
    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression) => SubQueryProcessor.ProcessSubQuery(this, methodCallExpression)
            ?? base.VisitMethodCall(methodCallExpression);
}
