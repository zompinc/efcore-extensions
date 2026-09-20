namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Pushes a projection containing a window function into a subquery when an operator that SQL evaluates before the window function follows it.
/// </summary>
/// <remarks>
/// Has to run before navigation expansion, which moves every projection to the end of the query and with it the information that the projection came first.
/// </remarks>
internal sealed class WindowFunctionProjectionDetector : ExpressionVisitor
{
    /// <summary>
    /// Operators translated to an aggregate, GROUP BY or WHERE. The first two cannot contain a window function, and WHERE is applied before it is computed.
    /// </summary>
    private static readonly FrozenSet<string> OperatorsAfterProjection =
    [
        nameof(Queryable.Average),
        nameof(Queryable.GroupBy),
        nameof(Queryable.Max),
        nameof(Queryable.Min),
        nameof(Queryable.Sum),
        nameof(Queryable.Where),
    ];

    /// <summary>
    /// Operators that only filter in their overload with a predicate. Any, All and Count are left out:
    /// they don't return the window function, so their result is the same whether or not it is computed first.
    /// </summary>
    private static readonly FrozenSet<string> OperatorsWithOptionalPredicate =
    [
        nameof(Queryable.First),
        nameof(Queryable.FirstOrDefault),
        nameof(Queryable.Last),
        nameof(Queryable.LastOrDefault),
        nameof(Queryable.Single),
        nameof(Queryable.SingleOrDefault),
    ];

    private static readonly FrozenSet<string> ProjectingOperators =
    [
        nameof(Queryable.GroupJoin),
        nameof(Queryable.Join),
        nameof(Queryable.Select),
        nameof(Queryable.SelectMany),
    ];

    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        var visited = (MethodCallExpression)base.VisitMethodCall(node);

        if (visited.Method.DeclaringType != typeof(Queryable)
            || !IsEvaluatedBeforeWindowFunction(visited)
            || !ProjectsWindowFunction(visited.Arguments[0]))
        {
            return visited;
        }

        var source = visited.Arguments[0];
        var elementType = source.Type.GetGenericArguments()[0];
        var asSubQueryMethod = WindowFunctionsEvaluatableExpressionFilter.AsSubQueryMethod.MakeGenericMethod(elementType);

        return visited.Update(null, [Expression.Call(null, asSubQueryMethod, source), .. visited.Arguments.Skip(1)]);
    }

    private static bool IsEvaluatedBeforeWindowFunction(MethodCallExpression call)
        => OperatorsAfterProjection.Contains(call.Method.Name)
            || (OperatorsWithOptionalPredicate.Contains(call.Method.Name)
                && call.Arguments.Skip(1).Any(a => a is UnaryExpression { NodeType: ExpressionType.Quote }));

    /// <summary>
    /// Follows the source of an operator down to the closest projection and checks it for a window function.
    /// </summary>
    private static bool ProjectsWindowFunction(Expression source)
    {
        while (source is MethodCallExpression { Arguments.Count: > 0 } call
            && call.Arguments[0].Type.IsGenericType
            && typeof(IQueryable).IsAssignableFrom(call.Arguments[0].Type))
        {
            if (call.Method.Name == nameof(DbFunctionsExtensions.AsSubQuery))
            {
                return false;
            }

            if (call.Method.DeclaringType == typeof(Queryable) && ProjectingOperators.Contains(call.Method.Name))
            {
                var detector = new WindowFunctionDetectorInternal();
                foreach (var argument in call.Arguments.Skip(1))
                {
                    _ = detector.Visit(argument);
                }

                if (detector.WindowFunctionsCollection.Count > 0)
                {
                    return true;
                }
            }

            source = call.Arguments[0];
        }

        return false;
    }
}