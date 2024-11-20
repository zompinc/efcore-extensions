namespace Zomp.EFCore.WindowFunctions.Sqlite.Query.Internal;

/// <summary>
/// The WindowFunctionsSqliteQueryableMethodTranslatingExpressionVisitorFactory.
/// </summary>
/// <param name="dependencies">Type mapping source dependencies.</param>
/// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
public class WindowFunctionsSqliteQueryableMethodTranslatingExpressionVisitorFactory(QueryableMethodTranslatingExpressionVisitorDependencies dependencies, RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies)
        : SqliteQueryableMethodTranslatingExpressionVisitorFactory(dependencies, relationalDependencies)
{
    private readonly QueryableMethodTranslatingExpressionVisitorDependencies dependencies = dependencies;
    private readonly RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies = relationalDependencies;

    /// <inheritdoc/>
    public override QueryableMethodTranslatingExpressionVisitor Create(QueryCompilationContext queryCompilationContext)

#if !EF_CORE_8
        => new WindowFunctionsSqliteQueryableMethodTranslatingExpressionVisitor(dependencies, relationalDependencies, (RelationalQueryCompilationContext)queryCompilationContext);
#else
        => new WindowFunctionsSqliteQueryableMethodTranslatingExpressionVisitor(dependencies, relationalDependencies, queryCompilationContext);
#endif
}