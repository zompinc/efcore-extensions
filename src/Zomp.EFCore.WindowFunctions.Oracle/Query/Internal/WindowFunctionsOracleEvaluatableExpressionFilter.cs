namespace Zomp.EFCore.WindowFunctions.Oracle.Query.Internal;

/// <summary>
/// The evaluatable expression filter, which keeps window functions from being evaluated on the client.
/// </summary>
/// <param name="dependencies">Service dependencies.</param>
/// <param name="relationalDependencies">Relational service dependencies.</param>
public class WindowFunctionsOracleEvaluatableExpressionFilter(EvaluatableExpressionFilterDependencies dependencies, RelationalEvaluatableExpressionFilterDependencies relationalDependencies)
    : OracleEvaluatableExpressionFilter(dependencies, relationalDependencies)
{
    /// <inheritdoc/>
    public override bool IsEvaluatableExpression(Expression expression, IModel model)
        => WindowFunctionsEvaluatableExpressionFilter.IsEvaluatableExpression(expression)
            && base.IsEvaluatableExpression(expression, model);
}