namespace Zomp.EFCore.WindowFunctions.Oracle.Query.Internal;

/// <summary>
/// The WindowFunctionsOracleQueryableMethodTranslatingExpressionVisitor.
/// </summary>
public class WindowFunctionsOracleQueryableMethodTranslatingExpressionVisitor : OracleQueryableMethodTranslatingExpressionVisitor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsOracleQueryableMethodTranslatingExpressionVisitor"/> class.
    /// </summary>
    /// <param name="dependencies">Type mapping source dependencies.</param>
    /// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
    /// <param name="queryCompilationContext">The query compilation context object to use.</param>
    public WindowFunctionsOracleQueryableMethodTranslatingExpressionVisitor(QueryableMethodTranslatingExpressionVisitorDependencies dependencies, RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies, QueryCompilationContext queryCompilationContext)
        : base(dependencies, relationalDependencies, queryCompilationContext)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsOracleQueryableMethodTranslatingExpressionVisitor"/> class.
    /// </summary>
    /// <param name="parentVisitor">The parent visitor.</param>
    protected WindowFunctionsOracleQueryableMethodTranslatingExpressionVisitor(OracleQueryableMethodTranslatingExpressionVisitor parentVisitor)
        : base(parentVisitor)
    {
    }

    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
    {
        return Extensions.SubQueryProcessor.ProcessSubQuery(this, methodCallExpression)
            ?? base.VisitMethodCall(methodCallExpression);
    }
}