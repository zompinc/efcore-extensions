namespace Zomp.EFCore.WindowFunctions.Sqlite.Query.Internal;

/// <summary>
/// Query SQL generator for Sqlite which includes window functions operations.
/// </summary>
public class SqliteWindowFunctionsEvaluatableExpressionFilter : RelationalEvaluatableExpressionFilter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteWindowFunctionsEvaluatableExpressionFilter"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="relationalDependencies">Relational dependencies.</param>
    public SqliteWindowFunctionsEvaluatableExpressionFilter(EvaluatableExpressionFilterDependencies dependencies, RelationalEvaluatableExpressionFilterDependencies relationalDependencies)
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
