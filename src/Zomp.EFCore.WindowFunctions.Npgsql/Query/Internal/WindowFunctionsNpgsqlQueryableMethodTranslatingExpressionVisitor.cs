namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// The WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitor.
/// </summary>
public class WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitor : NpgsqlQueryableMethodTranslatingExpressionVisitor
{
#if !EF_CORE_8
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitor"/> class.
    /// </summary>
    /// <param name="dependencies">Type mapping source dependencies.</param>
    /// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
    /// <param name="queryCompilationContext">The query compilation context object to use.</param>
    /// <param name="npgsqlSingletonOptions">NpgSql Singleton Options.</param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "EF Core 8")]
    public WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitor(
        QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
        RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
        RelationalQueryCompilationContext queryCompilationContext,
        INpgsqlSingletonOptions npgsqlSingletonOptions)
        : base(dependencies, relationalDependencies, queryCompilationContext, npgsqlSingletonOptions)
    {
    }
#else
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitor"/> class.
    /// </summary>
    /// <param name="dependencies">Type mapping source dependencies.</param>
    /// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
    /// <param name="queryCompilationContext">The query compilation context object to use.</param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "EF Core 8")]
    public WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitor(QueryableMethodTranslatingExpressionVisitorDependencies dependencies, RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies, QueryCompilationContext queryCompilationContext)
        : base(dependencies, relationalDependencies, queryCompilationContext)
    {
    }
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitor"/> class.
    /// </summary>
    /// <param name="parentVisitor">The visitor translating the query this subquery belongs to.</param>
    protected WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitor(WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitor parentVisitor)
        : base(parentVisitor)
    {
    }

    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression) => SubQueryProcessor.ProcessSubQuery(this, methodCallExpression)
            ?? base.VisitMethodCall(methodCallExpression);

    /// <inheritdoc/>
    protected override QueryableMethodTranslatingExpressionVisitor CreateSubqueryVisitor()
        => new WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitor(this);
}