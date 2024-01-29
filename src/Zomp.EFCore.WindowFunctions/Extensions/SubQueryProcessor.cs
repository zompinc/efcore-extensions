namespace Zomp.EFCore.WindowFunctions.Extensions;

/// <summary>
/// Subquery processor.
/// </summary>
public static class SubQueryProcessor
{
    /// <summary>
    /// Processes subquery.
    /// </summary>
    /// <param name="visitor">Expression visitor to use.</param>
    /// <param name="methodCallExpression">Method to check.</param>
    /// <returns>Method which will result in the query being pushed down.</returns>
    public static Expression? ProcessSubQuery(ExpressionVisitor visitor, MethodCallExpression methodCallExpression)
    {
        ArgumentNullException.ThrowIfNull(methodCallExpression);
        ArgumentNullException.ThrowIfNull(visitor);

        if (methodCallExpression.Method.Name == nameof(DbFunctionsExtensions.AsSubQuery))
        {
            var expression = visitor.Visit(methodCallExpression.Arguments[0]);

            if (expression is ShapedQueryExpression { QueryExpression: SelectExpression { } select } shapedQueryExpression)
            {
                select.PushdownIntoSubquery();
                return shapedQueryExpression;
            }
        }

        return null;
    }
}
