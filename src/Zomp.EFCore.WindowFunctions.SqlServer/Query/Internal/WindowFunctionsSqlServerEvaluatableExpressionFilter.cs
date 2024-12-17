namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// Filters which methods avoid client evalutation.
/// </summary>
/// <param name="dependencies">Service dependencies.</param>
/// <param name="relationalDependencies">Relational dependencies.</param>
public class WindowFunctionsSqlServerEvaluatableExpressionFilter(EvaluatableExpressionFilterDependencies dependencies, RelationalEvaluatableExpressionFilterDependencies relationalDependencies)
    : SqlServerEvaluatableExpressionFilter(dependencies, relationalDependencies)
{
    /// <inheritdoc/>
    public override bool IsEvaluatableExpression(Expression expression, IModel model)
        => WindowFunctionsEvaluatableExpressionFilter.IsEvaluatableExpression(expression)
            && base.IsEvaluatableExpression(expression, model);
}
