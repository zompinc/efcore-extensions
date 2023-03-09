namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// Evaluatable expression filter for Npgsql.
/// </summary>
public class WindowFunctionsNpgsqlEvaluatableExpressionFilter : NpgsqlEvaluatableExpressionFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlEvaluatableExpressionFilter"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="relationalDependencies">Relational service dependencies.</param>
    public WindowFunctionsNpgsqlEvaluatableExpressionFilter(EvaluatableExpressionFilterDependencies dependencies, RelationalEvaluatableExpressionFilterDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
    {
    }

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
