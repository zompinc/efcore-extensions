namespace Zomp.EFCore.WindowFunctions.Sqlite.Query.Internal;

/// <summary>
/// Query SQL generator for Sqlite which includes window functions operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SqliteWindowFunctionsEvaluatableExpressionFilter"/> class.
/// </remarks>
/// <param name="dependencies">Service dependencies.</param>
/// <param name="relationalDependencies">Relational dependencies.</param>
public class SqliteWindowFunctionsEvaluatableExpressionFilter(EvaluatableExpressionFilterDependencies dependencies, RelationalEvaluatableExpressionFilterDependencies relationalDependencies)
    : RelationalEvaluatableExpressionFilter(dependencies, relationalDependencies)
{
    /// <inheritdoc/>
    public override bool IsEvaluatableExpression(Expression expression, IModel model)
        => WindowFunctionsEvaluatableExpressionFilter.IsEvaluatableExpression(expression)
            && base.IsEvaluatableExpression(expression, model);
}
