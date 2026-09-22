namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Rewrites aggregates of correlated subqueries in a projection to window functions: a Count, LongCount, Sum, Min, Max or Average
/// of the rows sharing a key with the current row becomes the aggregate over a partition by that key.
/// </summary>
/// <remarks>
/// Only a subquery that reads exactly the rows the projection reads, filters included, is rewritten, since a window only sees
/// those rows, and only when it is correlated by equalities of the same expression on both sides, <c>t.Key == r.Key</c>. A
/// subquery that is anything else stays a correlated subquery, which is slower but gives the same result.
/// </remarks>
internal sealed class CorrelatedWindowExpressionVisitor : ExpressionVisitor
{
    private static readonly MethodInfo SelectMethod = typeof(Queryable).GetMethods()
        .Single(m => m.Name == nameof(Queryable.Select)
            && m.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments().Length == 2);

    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        var visited = (MethodCallExpression)base.VisitMethodCall(node);

        if (!visited.Method.IsGenericMethod
            || visited.Method.GetGenericMethodDefinition() != SelectMethod
            || visited.Arguments is not [var source, UnaryExpression { Operand: LambdaExpression selector }])
        {
            return visited;
        }

        var replacer = new SubqueryReplacer(source, selector.Parameters[0]);
        var body = replacer.Visit(selector.Body);

        return replacer.Replaced
            ? visited.Update(null, [source, Expression.Quote(Expression.Lambda(body, selector.Parameters[0]))])
            : visited;
    }

    /// <summary>
    /// Replaces the correlated aggregates over <paramref name="source"/> with window functions over the current row,
    /// <paramref name="row"/>.
    /// </summary>
    private sealed class SubqueryReplacer(Expression source, ParameterExpression row) : ExpressionVisitor
    {
        /// <summary>
        /// Gets a value indicating whether any subquery was replaced.
        /// </summary>
        public bool Replaced { get; private set; }

        /// <inheritdoc/>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (Window(node) is { } window)
            {
                Replaced = true;
                return window;
            }

            return base.VisitMethodCall(node);
        }

        private static IEnumerable<Expression> Conjuncts(Expression predicate)
            => predicate is BinaryExpression { NodeType: ExpressionType.AndAlso } and
                ? Conjuncts(and.Left).Concat(Conjuncts(and.Right))
                : [predicate];

        /// <summary>
        /// Removes the OrderBy and ThenBy calls that do not decide which rows a query returns: those above its filters, before any
        /// operator such as Take that makes the order matter.
        /// </summary>
        private static Expression WithoutOrdering(Expression query)
        {
            if (query is not MethodCallExpression { Method: var method } call || method.DeclaringType != typeof(Queryable))
            {
                return query;
            }

            if (method.Name is nameof(Queryable.OrderBy) or nameof(Queryable.OrderByDescending)
                or nameof(Queryable.ThenBy) or nameof(Queryable.ThenByDescending))
            {
                return WithoutOrdering(call.Arguments[0]);
            }

            if (method.Name != nameof(Queryable.Where))
            {
                return query;
            }

            var filtered = WithoutOrdering(call.Arguments[0]);
            return filtered == call.Arguments[0] ? call : call.Update(null, [filtered, call.Arguments[1]]);
        }

        private static bool References(Expression expression, ParameterExpression parameter)
        {
            var finder = new ParameterFinder(parameter);
            _ = finder.Visit(expression);
            return finder.Found;
        }

        private Expression? Window(MethodCallExpression call)
        {
            if (call.Method.DeclaringType != typeof(Queryable))
            {
                return null;
            }

            var predicates = new List<LambdaExpression>();
            LambdaExpression? value = null;
            switch (call)
            {
                case { Method.Name: nameof(Queryable.Count) or nameof(Queryable.LongCount), Arguments: [_] }:
                    break;
                case { Method.Name: nameof(Queryable.Count) or nameof(Queryable.LongCount), Arguments: [_, UnaryExpression { Operand: LambdaExpression predicate }] }:
                    predicates.Add(predicate);
                    break;
                case { Method.Name: nameof(Queryable.Sum) or nameof(Queryable.Min) or nameof(Queryable.Max) or nameof(Queryable.Average), Arguments: [_, UnaryExpression { Operand: LambdaExpression selector }] }:
                    value = selector;
                    break;
                default:
                    return null;
            }

            var inner = call.Arguments[0];
            while (inner is MethodCallExpression { Method: { Name: nameof(Queryable.Where) } where } whereCall
                && where.DeclaringType == typeof(Queryable)
                && whereCall.Arguments[1] is UnaryExpression { Operand: LambdaExpression { Parameters.Count: 1 } filter }
                && References(filter.Body, row))
            {
                predicates.Add(filter);
                inner = whereCall.Arguments[0];
            }

            if (predicates.Count == 0 || !ExpressionEqualityComparer.Instance.Equals(WithoutOrdering(inner), WithoutOrdering(source)))
            {
                return null;
            }

            var keys = new List<Expression>();
            foreach (var predicate in predicates)
            {
                foreach (var conjunct in Conjuncts(predicate.Body))
                {
                    if (Key(conjunct, predicate.Parameters[0]) is not { } key)
                    {
                        return null;
                    }

                    keys.Add(key);
                }
            }

            var over = OverClauseBuilder.Build(row, keys, [], requireOrderBy: false);

            return call.Method.Name switch
            {
                nameof(Queryable.Count) => WindowAggregates.Count(over),
                nameof(Queryable.LongCount) => WindowAggregates.LongCount(over),
                _ => WindowAggregates.Aggregate(
                    call.Method.Name,
                    ReplacingExpressionVisitor.Replace(value!.Parameters[0], row, value.Body),
                    call.Type,
                    over),
            };
        }

        /// <summary>
        /// Recognizes <c>t.Key == r.Key</c>, in either order, where both sides are the same expression of their row.
        /// </summary>
        /// <returns>The key of the current row, or <see langword="null"/> for anything else.</returns>
        private Expression? Key(Expression conjunct, ParameterExpression other)
        {
            if (conjunct is not BinaryExpression { NodeType: ExpressionType.Equal } equal)
            {
                return null;
            }

            var (otherSide, rowSide) = (References(equal.Left, other), References(equal.Left, row), References(equal.Right, other), References(equal.Right, row)) switch
            {
                (true, false, false, true) => (equal.Left, equal.Right),
                (false, true, true, false) => (equal.Right, equal.Left),
                _ => (null, null),
            };

            return otherSide is not null
                && ExpressionEqualityComparer.Instance.Equals(ReplacingExpressionVisitor.Replace(other, row, otherSide), rowSide)
                ? rowSide
                : null;
        }
    }

    private sealed class ParameterFinder(ParameterExpression parameter) : ExpressionVisitor
    {
        public bool Found { get; private set; }

        /// <inheritdoc/>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            Found |= node == parameter;
            return node;
        }
    }
}
