namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// Evaluatable expression filter for Npgsql.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlEvaluatableExpressionFilter"/> class.
/// </remarks>
/// <param name="dependencies">Service dependencies.</param>
/// <param name="relationalDependencies">Relational service dependencies.</param>
/// <param name="npgsqlSingletonOptions">NpgSql Singleton Options.</param>
public class WindowFunctionsNpgsqlEvaluatableExpressionFilter(EvaluatableExpressionFilterDependencies dependencies, RelationalEvaluatableExpressionFilterDependencies relationalDependencies, INpgsqlSingletonOptions npgsqlSingletonOptions)
    : NpgsqlEvaluatableExpressionFilter(dependencies, relationalDependencies, npgsqlSingletonOptions)
{
    /// <inheritdoc/>
    public override bool IsEvaluatableExpression(Expression expression, IModel model)
        => WindowFunctionsEvaluatableExpressionFilter.IsEvaluatableExpression(expression)
        && base.IsEvaluatableExpression(expression, model);
}
