namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// The WindowFunctionsRelationalQueryTranslationPreprocessor.
/// </summary>
/// <param name="dependencies">Type mapping source dependencies.</param>
/// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
/// <param name="queryCompilationContext">The query compilation context object to use.</param>
public class WindowFunctionsRelationalQueryTranslationPreprocessor(QueryTranslationPreprocessorDependencies dependencies, RelationalQueryTranslationPreprocessorDependencies relationalDependencies, QueryCompilationContext queryCompilationContext) : RelationalQueryTranslationPreprocessor(dependencies, relationalDependencies, queryCompilationContext)
{
    /// <inheritdoc/>
    public override Expression Process(Expression query)
    {
        query = new InvocationExpressionRemovingExpressionVisitor().Visit(query);
        query = NormalizeQueryableMethod(query);
        query = new CallForwardingExpressionVisitor().Visit(query);
        query = new NullCheckRemovingExpressionVisitor().Visit(query);
        query = new SubqueryMemberPushdownExpressionVisitor(QueryCompilationContext.Model).Visit(query);
        query = new JoinDetector().Visit(query);
        query = new NavigationExpandingExpressionVisitor(
                this,
                QueryCompilationContext,
                Dependencies.EvaluatableExpressionFilter,
                Dependencies.NavigationExpansionExtensibilityHelper)
            .Expand(query);
        query = new QueryOptimizingExpressionVisitor().Visit(query);
        query = new NullCheckRemovingExpressionVisitor().Visit(query);
        query = new WindowFunctionInsideWhereDetector().Visit(query);
        return query;
    }
}
