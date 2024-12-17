namespace Zomp.EFCore.WindowFunctions.Sqlite.Query.Internal;

/// <summary>
/// The WindowFunctionsSqliteQueryableMethodTranslatingExpressionVisitor.
/// </summary>
/// <param name="dependencies">Type mapping source dependencies.</param>
/// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
/// <param name="queryCompilationContext">The query compilation context object to use.</param>
public class WindowFunctionsSqliteQueryableMethodTranslatingExpressionVisitor
    (QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
    RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
#if !EF_CORE_8
    RelationalQueryCompilationContext queryCompilationContext)
#else
    QueryCompilationContext queryCompilationContext)
#endif
    : SqliteQueryableMethodTranslatingExpressionVisitor(dependencies, relationalDependencies, queryCompilationContext)
{
    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression) => SubQueryProcessor.ProcessSubQuery(this, methodCallExpression)
            ?? base.VisitMethodCall(methodCallExpression);
}