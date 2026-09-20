namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Pushes a query into a subquery where SQL would otherwise evaluate an operator and a window function in the opposite order to LINQ:
/// a projection with a window function followed by a filter, aggregate or GROUP BY, and a row limit followed by a window function.
/// </summary>
/// <remarks>
/// Has to run before navigation expansion, which moves every projection to the end of the query and with it the information that the projection came first.
/// </remarks>
internal sealed class WindowFunctionProjectionDetector : ExpressionVisitor
{
    /// <summary>
    /// Operators translated to an aggregate, GROUP BY, WHERE or a join. The first two cannot contain a window function,
    /// and the other two are applied before it is computed.
    /// </summary>
    /// <remarks>
    /// LeftJoin and RightJoin are spelled out because Queryable only has them from .NET 10.
    /// </remarks>
    private static readonly FrozenSet<string> OperatorsAfterProjection =
        new[]
        {
            nameof(Queryable.Average),
            nameof(Queryable.GroupBy),
            nameof(Queryable.GroupJoin),
            nameof(Queryable.Join),
            "LeftJoin",
            nameof(Queryable.Max),
            nameof(Queryable.Min),
            "RightJoin",
            nameof(Queryable.SelectMany),
            nameof(Queryable.Sum),
            nameof(Queryable.Where),
        }.ToFrozenSet();

    /// <summary>
    /// Operators that only filter in their overload with a predicate. Any, All and Count are left out:
    /// they don't return the window function, so their result is the same whether or not it is computed first.
    /// </summary>
    private static readonly FrozenSet<string> OperatorsWithOptionalPredicate =
        new[]
        {
            nameof(Queryable.First),
            nameof(Queryable.FirstOrDefault),
            nameof(Queryable.Last),
            nameof(Queryable.LastOrDefault),
            nameof(Queryable.Single),
            nameof(Queryable.SingleOrDefault),
        }.ToFrozenSet();

    /// <summary>
    /// Operators SQL applies after the window functions of the same SELECT.
    /// </summary>
    private static readonly FrozenSet<string> RowLimitingOperators =
        new[]
        {
            nameof(Queryable.Skip),
            nameof(Queryable.Take),
        }.ToFrozenSet();

    /// <summary>
    /// Operators a row limit stays in the same SELECT with.
    /// </summary>
    private static readonly FrozenSet<string> OperatorsKeepingRowLimit =
        new[]
        {
            nameof(Queryable.OrderBy),
            nameof(Queryable.OrderByDescending),
            nameof(Queryable.Select),
            nameof(Queryable.ThenBy),
            nameof(Queryable.ThenByDescending),
        }.ToFrozenSet();

    private static readonly FrozenSet<string> ProjectingOperators =
        new[]
        {
            nameof(Queryable.GroupJoin),
            nameof(Queryable.Join),
            nameof(Queryable.Select),
            nameof(Queryable.SelectMany),
        }.ToFrozenSet();

    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        var visited = (MethodCallExpression)base.VisitMethodCall(node);

        if (visited.Method.DeclaringType != typeof(Queryable))
        {
            return visited;
        }

        var needsSubquery = IsEvaluatedBeforeWindowFunction(visited)
            ? ProjectsWindowFunction(visited.Arguments[0])
            : ProjectingOperators.Contains(visited.Method.Name)
                && ContainsWindowFunction(visited)
                && IsRowLimited(visited.Arguments[0]);

        if (!needsSubquery)
        {
            return visited;
        }

        var source = visited.Arguments[0];
        var elementType = source.Type.GetGenericArguments()[0];
        var asSubQueryMethod = WindowFunctionsEvaluatableExpressionFilter.AsSubQueryMethod.MakeGenericMethod(elementType);

        return visited.Update(null, [Expression.Call(null, asSubQueryMethod, source), .. visited.Arguments.Skip(1)]);
    }

    private static bool ContainsWindowFunction(MethodCallExpression call)
    {
        var detector = new WindowFunctionDetectorInternal();
        foreach (var argument in call.Arguments.Skip(1))
        {
            _ = detector.Visit(argument);
        }

        return detector.WindowFunctionsCollection.Count > 0;
    }

    /// <summary>
    /// Checks whether Skip or Take ends up in the same SELECT as a projection over <paramref name="source"/>.
    /// </summary>
    private static bool IsRowLimited(Expression source)
    {
        while (source is MethodCallExpression { Method: var method } call && method.DeclaringType == typeof(Queryable))
        {
            if (RowLimitingOperators.Contains(method.Name))
            {
                return true;
            }

            if (!OperatorsKeepingRowLimit.Contains(method.Name))
            {
                return false;
            }

            source = call.Arguments[0];
        }

        return false;
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

            if (call.Method.DeclaringType == typeof(Queryable)
                && ProjectingOperators.Contains(call.Method.Name)
                && ContainsWindowFunction(call))
            {
                return true;
            }

            source = call.Arguments[0];
        }

        return false;
    }
}