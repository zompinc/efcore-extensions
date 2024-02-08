namespace Zomp.EFCore.WindowFunctions.SqlServer;

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
    {
        if (!WindowFunctionsEvaluatableExpressionFilter.IsEvaluatableExpression(expression))
        {
            return false;
        }

        return base.IsEvaluatableExpression(expression, model);
    }
}
