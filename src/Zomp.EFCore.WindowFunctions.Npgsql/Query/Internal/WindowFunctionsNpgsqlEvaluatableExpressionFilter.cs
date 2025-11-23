namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// Evaluatable expression filter for Npgsql.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlEvaluatableExpressionFilter"/> class.
/// </remarks>
public class WindowFunctionsNpgsqlEvaluatableExpressionFilter : NpgsqlEvaluatableExpressionFilter
{
#if !EF_CORE_8 && !EF_CORE_9
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlEvaluatableExpressionFilter"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="relationalDependencies">Relational service dependencies.</param>
    /// <param name="npgsqlSingletonOptions">NpgSql Singleton Options.</param>
    public WindowFunctionsNpgsqlEvaluatableExpressionFilter(EvaluatableExpressionFilterDependencies dependencies, RelationalEvaluatableExpressionFilterDependencies relationalDependencies, INpgsqlSingletonOptions npgsqlSingletonOptions)
        : base(dependencies, relationalDependencies, npgsqlSingletonOptions)
    {
    }
#else
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlEvaluatableExpressionFilter"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="relationalDependencies">Relational service dependencies.</param>
    public WindowFunctionsNpgsqlEvaluatableExpressionFilter(EvaluatableExpressionFilterDependencies dependencies, RelationalEvaluatableExpressionFilterDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
    {
    }
#endif

    /// <inheritdoc/>
    public override bool IsEvaluatableExpression(Expression expression, IModel model)
        => WindowFunctionsEvaluatableExpressionFilter.IsEvaluatableExpression(expression)
        && base.IsEvaluatableExpression(expression, model);
}
