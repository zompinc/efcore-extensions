namespace Zomp.EFCore.WindowFunctions.Sqlite.Query.Internal;

/// <summary>
/// The WindowFunctionsSqliteQueryableMethodTranslatingExpressionVisitor.
/// </summary>
public class WindowFunctionsSqliteQueryableMethodTranslatingExpressionVisitor : SqliteQueryableMethodTranslatingExpressionVisitor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqliteQueryableMethodTranslatingExpressionVisitor"/> class.
    /// </summary>
    /// <param name="dependencies">Type mapping source dependencies.</param>
    /// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
    /// <param name="queryCompilationContext">The query compilation context object to use.</param>
    public WindowFunctionsSqliteQueryableMethodTranslatingExpressionVisitor(
        QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
        RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
#if !EF_CORE_8
        RelationalQueryCompilationContext queryCompilationContext)
#else
        QueryCompilationContext queryCompilationContext)
#endif
        : base(dependencies, relationalDependencies, queryCompilationContext)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqliteQueryableMethodTranslatingExpressionVisitor"/> class.
    /// </summary>
    /// <param name="parentVisitor">The visitor translating the query this subquery belongs to.</param>
    protected WindowFunctionsSqliteQueryableMethodTranslatingExpressionVisitor(WindowFunctionsSqliteQueryableMethodTranslatingExpressionVisitor parentVisitor)
        : base(parentVisitor)
    {
    }

    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression) => SubQueryProcessor.ProcessSubQuery(this, methodCallExpression)
            ?? base.VisitMethodCall(methodCallExpression);

    /// <inheritdoc/>
    protected override QueryableMethodTranslatingExpressionVisitor CreateSubqueryVisitor()
        => new WindowFunctionsSqliteQueryableMethodTranslatingExpressionVisitor(this);
}