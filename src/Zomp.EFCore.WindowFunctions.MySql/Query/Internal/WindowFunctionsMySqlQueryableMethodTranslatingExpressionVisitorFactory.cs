namespace Zomp.EFCore.WindowFunctions.MySql.Query.Internal;

/// <summary>
/// Factory for creating <see cref="WindowFunctionsMySqlQueryableMethodTranslatingExpressionVisitor"/> instances.
/// </summary>
/// <param name="dependencies">Service dependencies.</param>
/// <param name="relationalDependencies">Relational service dependencies.</param>
/// <param name="options">MySQL options.</param>
public class WindowFunctionsMySqlQueryableMethodTranslatingExpressionVisitorFactory(QueryableMethodTranslatingExpressionVisitorDependencies dependencies, RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies, IMySqlOptions options)
    : IQueryableMethodTranslatingExpressionVisitorFactory
{
    /// <inheritdoc/>
    public QueryableMethodTranslatingExpressionVisitor Create(QueryCompilationContext queryCompilationContext)
        => new WindowFunctionsMySqlQueryableMethodTranslatingExpressionVisitor(dependencies, relationalDependencies, (RelationalQueryCompilationContext)queryCompilationContext, options);
}