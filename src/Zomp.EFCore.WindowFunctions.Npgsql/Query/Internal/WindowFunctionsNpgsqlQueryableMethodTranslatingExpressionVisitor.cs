namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// The WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitor.
/// </summary>
/// <param name="dependencies">Type mapping source dependencies.</param>
/// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
/// <param name="queryCompilationContext">The query compilation context object to use.</param>
public class WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitor(QueryableMethodTranslatingExpressionVisitorDependencies dependencies, RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies, QueryCompilationContext queryCompilationContext) : NpgsqlQueryableMethodTranslatingExpressionVisitor(dependencies, relationalDependencies, queryCompilationContext)
{
    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
    {
        return SubQueryProcessor.ProcessSubQuery(this, methodCallExpression)
            ?? base.VisitMethodCall(methodCallExpression);
    }
}