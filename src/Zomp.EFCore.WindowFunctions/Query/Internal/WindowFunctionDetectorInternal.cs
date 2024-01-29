namespace Zomp.EFCore.WindowFunctions.Query.Internal;

internal sealed class WindowFunctionDetectorInternal : ExpressionVisitor
{
    public IList<MethodCallExpression> WindowFunctionsCollection { get; } = [];

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (WindowFunctionsEvaluatableExpressionFilter.WindowFunctionMethods.Contains(node.Method, CompareNameAndDeclaringType.Default))
        {
            WindowFunctionsCollection.Add(node);
        }

        return base.VisitMethodCall(node);
    }
}
