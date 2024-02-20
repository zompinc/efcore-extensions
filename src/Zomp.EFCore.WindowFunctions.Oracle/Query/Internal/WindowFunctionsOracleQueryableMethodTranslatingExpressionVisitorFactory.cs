namespace Zomp.EFCore.WindowFunctions.Oracle.Query.Internal;

/// <summary>
/// The WindowFunctionsOracleQueryableMethodTranslatingExpressionVisitorFactory.
/// </summary>
/// <param name="dependencies">Type mapping source dependencies.</param>
/// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
public class WindowFunctionsOracleQueryableMethodTranslatingExpressionVisitorFactory(QueryableMethodTranslatingExpressionVisitorDependencies dependencies, RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies)
    : OracleQueryableMethodTranslatingExpressionVisitorFactory(dependencies, relationalDependencies)
{
    private readonly QueryableMethodTranslatingExpressionVisitorDependencies dependencies = dependencies;
    private readonly RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies = relationalDependencies;

    /// <inheritdoc/>
    public override QueryableMethodTranslatingExpressionVisitor Create(QueryCompilationContext queryCompilationContext)
    {
        return new WindowFunctionsOracleQueryableMethodTranslatingExpressionVisitor(
            dependencies, relationalDependencies, queryCompilationContext);
    }
}