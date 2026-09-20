namespace Zomp.EFCore.WindowFunctions.MySql.Query.Internal;

/// <summary>
/// The evaluatable expression filter, which keeps window functions from being evaluated on the client.
/// </summary>
/// <param name="dependencies">Service dependencies.</param>
/// <param name="relationalDependencies">Relational service dependencies.</param>
/// <param name="plugins">Pomelo's own evaluatable expression filter plugins.</param>
public class WindowFunctionsMySqlEvaluatableExpressionFilter(EvaluatableExpressionFilterDependencies dependencies, RelationalEvaluatableExpressionFilterDependencies relationalDependencies, IEnumerable<IMySqlEvaluatableExpressionFilter> plugins)
    : MySqlEvaluatableExpressionFilter(dependencies, relationalDependencies, plugins)
{
    /// <inheritdoc/>
    public override bool IsEvaluatableExpression(Expression expression, IModel model)
        => WindowFunctionsEvaluatableExpressionFilter.IsEvaluatableExpression(expression)
            && base.IsEvaluatableExpression(expression, model);
}