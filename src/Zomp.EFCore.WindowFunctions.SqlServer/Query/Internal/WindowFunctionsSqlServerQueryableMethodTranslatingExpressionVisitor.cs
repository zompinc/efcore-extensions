namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// The WindowFunctionsSqlServerQueryableMethodTranslatingExpressionVisitor.
/// </summary>
public class WindowFunctionsSqlServerQueryableMethodTranslatingExpressionVisitor : SqlServerQueryableMethodTranslatingExpressionVisitor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqlServerQueryableMethodTranslatingExpressionVisitor"/> class.
    /// </summary>
    /// <param name="dependencies">Type mapping source dependencies.</param>
    /// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
    /// <param name="queryCompilationContext">The query compilation context object to use.</param>
    /// <param name="sqlServerSingletonOptions">The singleton option.</param>
    public WindowFunctionsSqlServerQueryableMethodTranslatingExpressionVisitor(
        QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
        RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
        SqlServerQueryCompilationContext queryCompilationContext,
        ISqlServerSingletonOptions sqlServerSingletonOptions)
        : base(dependencies, relationalDependencies, queryCompilationContext, sqlServerSingletonOptions)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqlServerQueryableMethodTranslatingExpressionVisitor"/> class.
    /// </summary>
    /// <param name="parentVisitor">The visitor translating the query this subquery belongs to.</param>
    protected WindowFunctionsSqlServerQueryableMethodTranslatingExpressionVisitor(WindowFunctionsSqlServerQueryableMethodTranslatingExpressionVisitor parentVisitor)
        : base(parentVisitor)
    {
    }

    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression) => SubQueryProcessor.ProcessSubQuery(this, methodCallExpression)
            ?? base.VisitMethodCall(methodCallExpression);

    /// <inheritdoc/>
    protected override QueryableMethodTranslatingExpressionVisitor CreateSubqueryVisitor()
        => new WindowFunctionsSqlServerQueryableMethodTranslatingExpressionVisitor(this);
}