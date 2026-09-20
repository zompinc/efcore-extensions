namespace Zomp.EFCore.WindowFunctions.MySql.Query.Internal;

/// <summary>
/// The queryable method translating visitor, which pushes a query down where a window function needs a subquery.
/// </summary>
public class WindowFunctionsMySqlQueryableMethodTranslatingExpressionVisitor : MySqlQueryableMethodTranslatingExpressionVisitor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsMySqlQueryableMethodTranslatingExpressionVisitor"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="relationalDependencies">Relational service dependencies.</param>
    /// <param name="queryCompilationContext">The query compilation context object to use.</param>
    /// <param name="options">MySQL options.</param>
    public WindowFunctionsMySqlQueryableMethodTranslatingExpressionVisitor(
        QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
        RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
        RelationalQueryCompilationContext queryCompilationContext,
        IMySqlOptions options)
        : base(dependencies, relationalDependencies, queryCompilationContext, options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsMySqlQueryableMethodTranslatingExpressionVisitor"/> class.
    /// </summary>
    /// <param name="parentVisitor">The visitor translating the query this subquery belongs to.</param>
    protected WindowFunctionsMySqlQueryableMethodTranslatingExpressionVisitor(WindowFunctionsMySqlQueryableMethodTranslatingExpressionVisitor parentVisitor)
        : base(parentVisitor)
    {
    }

    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        => SubQueryProcessor.ProcessSubQuery(this, methodCallExpression) ?? base.VisitMethodCall(methodCallExpression);

    /// <inheritdoc/>
    protected override QueryableMethodTranslatingExpressionVisitor CreateSubqueryVisitor()
        => new WindowFunctionsMySqlQueryableMethodTranslatingExpressionVisitor(this);
}