namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Collects the window functions of an expression, except those inside an <see cref="DbFunctionsExtensions.AsSubQuery{TEntity}"/>:
/// they are computed in that subquery, whatever the query does with its rows afterwards.
/// </summary>
internal sealed class WindowFunctionDetectorInternal : ExpressionVisitor
{
    public IList<MethodCallExpression> WindowFunctionsCollection { get; } = [];

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (WindowFunctionsEvaluatableExpressionFilter.IsAsSubQuery(node.Method))
        {
            return node;
        }

        if (WindowFunctionsEvaluatableExpressionFilter.WindowFunctionMethods.Contains(node.Method, CompareNameAndDeclaringType.Default))
        {
            WindowFunctionsCollection.Add(node);
        }

        return base.VisitMethodCall(node);
    }
}
