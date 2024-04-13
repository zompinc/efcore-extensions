namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// The WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitorFactory.
/// </summary>
/// <param name="dependencies">Type mapping source dependencies.</param>
/// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
public class WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitorFactory(QueryableMethodTranslatingExpressionVisitorDependencies dependencies, RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies)
    : NpgsqlQueryableMethodTranslatingExpressionVisitorFactory(dependencies, relationalDependencies)
{
    private readonly QueryableMethodTranslatingExpressionVisitorDependencies dependencies = dependencies;
    private readonly RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies = relationalDependencies;

    /// <inheritdoc/>
    public override QueryableMethodTranslatingExpressionVisitor Create(QueryCompilationContext queryCompilationContext)
        => new WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitor(dependencies, relationalDependencies, queryCompilationContext);
}