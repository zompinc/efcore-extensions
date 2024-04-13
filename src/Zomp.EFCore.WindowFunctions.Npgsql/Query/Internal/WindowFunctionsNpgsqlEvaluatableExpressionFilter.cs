namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// Evaluatable expression filter for Npgsql.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlEvaluatableExpressionFilter"/> class.
/// </remarks>
/// <param name="dependencies">Service dependencies.</param>
/// <param name="relationalDependencies">Relational service dependencies.</param>
public class WindowFunctionsNpgsqlEvaluatableExpressionFilter(EvaluatableExpressionFilterDependencies dependencies, RelationalEvaluatableExpressionFilterDependencies relationalDependencies) : NpgsqlEvaluatableExpressionFilter(dependencies, relationalDependencies)
{
    /// <inheritdoc/>
    public override bool IsEvaluatableExpression(Expression expression, IModel model)
        => WindowFunctionsEvaluatableExpressionFilter.IsEvaluatableExpression(expression)
        && base.IsEvaluatableExpression(expression, model);
}
