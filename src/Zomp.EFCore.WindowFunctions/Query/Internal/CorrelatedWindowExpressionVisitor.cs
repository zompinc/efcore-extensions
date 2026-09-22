namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Rewrites aggregates of correlated subqueries in a projection to window functions: a Count, LongCount, Sum, Min, Max or Average
/// of the rows sharing a key with the current row becomes the aggregate over a partition by that key, and of the rows before it
/// in some order, a running aggregate or RANK.
/// </summary>
/// <remarks>
/// <para>
/// Only a subquery that reads exactly the rows the projection reads, filters included, is rewritten, since a window only sees
/// those rows, and only when it is correlated by equalities of the same expression on both sides, <c>t.Key == r.Key</c>, and at
/// most one comparison of a non-nullable expression, <c>t.Order &lt;= r.Order</c>. A subquery that is anything else stays a
/// correlated subquery, which is slower but gives the same result.
/// </para>
/// <para>
/// Up to and including the current row, <c>&lt;=</c> or <c>&gt;=</c>, is the default frame of an ordered window, since it
/// takes in the rows tied with the current one. Strictly before it, <c>&lt;</c> or <c>&gt;</c>, is only expressible for Count,
/// as RANK() - 1: with ties it is not a frame.
/// </para>
/// <para>
/// A value of the previous row, the first of the rows before the current one ordered down to it, becomes LAG, and of the next
/// row LEAD. Only when the order is unique, a key or unique index of the entity, is that row the one LAG reads.
/// </para>
/// </remarks>
/// <param name="model">The model, which says which properties are unique.</param>
internal sealed class CorrelatedWindowExpressionVisitor(IModel model) : ExpressionVisitor
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

        var replacer = new SubqueryReplacer(model, source, selector.Parameters[0]);
        var body = replacer.Visit(selector.Body);

        return replacer.Replaced
            ? visited.Update(null, [source, Expression.Quote(Expression.Lambda(body, selector.Parameters[0]))])
            : visited;
    }

    /// <summary>
    /// Replaces the correlated aggregates over <paramref name="source"/> with window functions over the current row,
    /// <paramref name="row"/>.
    /// </summary>
    private sealed class SubqueryReplacer(IModel model, Expression source, ParameterExpression row) : ExpressionVisitor
    {
        private static readonly MethodInfo RankMethod = typeof(DbFunctionsExtensions).GetMethods()
            .Single(m => m.Name == nameof(DbFunctionsExtensions.Rank) && !m.IsGenericMethod);

        /// <summary>
        /// Gets a value indicating whether any subquery was replaced.
        /// </summary>
        public bool Replaced { get; private set; }

        /// <inheritdoc/>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if ((Window(node) ?? Neighbour(node)) is { } window)
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
        /// Finds LAG or LEAD, a window function taking an expression, an offset and an over clause, for a value of
        /// <paramref name="valueType"/>.
        /// </summary>
        private static MethodInfo? NeighbourMethod(string name, Type valueType)
        {
            var underlying = Nullable.GetUnderlyingType(valueType);
            foreach (var candidate in typeof(DbFunctionsExtensions).GetMethods())
            {
                if (candidate.Name != name
                    || candidate.GetParameters() is not [_, { ParameterType: var parameter }, { ParameterType: var offset }, { ParameterType: var over }]
                    || offset != typeof(long)
                    || over != typeof(OverClause))
                {
                    continue;
                }

                if (!candidate.IsGenericMethodDefinition)
                {
                    if (parameter == valueType)
                    {
                        return candidate;
                    }

                    continue;
                }

                var takesNullable = parameter.IsGenericType && parameter.GetGenericTypeDefinition() == typeof(Nullable<>);
                if (candidate.GetGenericArguments().Length != 1 || takesNullable != (underlying is not null))
                {
                    continue;
                }

                try
                {
                    return candidate.MakeGenericMethod(underlying ?? valueType);
                }
                catch (ArgumentException)
                {
                    // The type does not meet the candidate's constraint, such as a class for a struct overload.
                }
            }

            return null;
        }

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

        /// <summary>
        /// Recognizes a value of the previous or next row: the rows before, or after, the current one in a unique order, ordered
        /// down, or up, to it, projected to the value, first or default.
        /// </summary>
        private Expression? Neighbour(MethodCallExpression call)
        {
            if (call is not { Method.Name: nameof(Queryable.FirstOrDefault), Arguments: [MethodCallExpression { Method.Name: nameof(Queryable.Select) } select] }
                || call.Method.DeclaringType != typeof(Queryable)
                || select.Method.DeclaringType != typeof(Queryable)
                || select.Arguments[1] is not UnaryExpression { Operand: LambdaExpression { Parameters.Count: 1 } value })
            {
                return null;
            }

            var predicates = new List<LambdaExpression>();
            var orderedBy = new List<(LambdaExpression Key, bool Descending)>();
            var inner = select.Arguments[0];
            while (inner is MethodCallExpression { Method: var method } innerCall && method.DeclaringType == typeof(Queryable))
            {
                if (method.Name == nameof(Queryable.Where)
                    && innerCall.Arguments[1] is UnaryExpression { Operand: LambdaExpression { Parameters.Count: 1 } filter }
                    && References(filter.Body, row))
                {
                    predicates.Add(filter);
                }
                else if (method.Name is nameof(Queryable.OrderBy) or nameof(Queryable.OrderByDescending)
                    && innerCall.Arguments[1] is UnaryExpression { Operand: LambdaExpression orderingKey })
                {
                    orderedBy.Add((orderingKey, method.Name == nameof(Queryable.OrderByDescending)));
                }
                else
                {
                    break;
                }

                inner = innerCall.Arguments[0];
            }

            if (orderedBy is not [var (orderKey, descending)]
                || !ExpressionEqualityComparer.Instance.Equals(WithoutOrdering(inner), WithoutOrdering(source)))
            {
                return null;
            }

            var keys = new List<Expression>();
            var comparisons = new List<(Expression Key, ExpressionType Comparison)>();
            foreach (var predicate in predicates)
            {
                foreach (var conjunct in Conjuncts(predicate.Body))
                {
                    switch (Correlation(conjunct, predicate.Parameters[0]))
                    {
                        case (var key, ExpressionType.Equal):
                            keys.Add(key);
                            break;
                        case { } comparison:
                            comparisons.Add(comparison);
                            break;
                        default:
                            return null;
                    }
                }
            }

            // LAG: the rows before the current one, nearest first. LEAD: the rows after it, nearest first.
            var (function, ordering) = (comparisons, descending) switch
            {
                ([(var key, ExpressionType.LessThan)], true) => (nameof(DbFunctionsExtensions.Lag), key),
                ([(var key, ExpressionType.GreaterThan)], false) => (nameof(DbFunctionsExtensions.Lead), key),
                _ => (null, null),
            };

            if (function is null
                || !ExpressionEqualityComparer.Instance.Equals(ReplacingExpressionVisitor.Replace(orderKey.Parameters[0], row, orderKey.Body), ordering)
                || !IsUnique(ordering))
            {
                return null;
            }

            var valueOfRow = ReplacingExpressionVisitor.Replace(value.Parameters[0], row, value.Body);
            if (NeighbourMethod(function, valueOfRow.Type) is not { } neighbourMethod)
            {
                return null;
            }

            var over = OverClauseBuilder.Build(row, keys, [(Expression.Lambda(ordering, row), false)], requireOrderBy: true);
            Expression result = Expression.Call(neighbourMethod, OverClauseBuilder.Functions, valueOfRow, Expression.Constant(1L), over);

            // FirstOrDefault gives the default of a non-nullable value when there is no such row, where LAG and LEAD give NULL.
            if (call.Type.IsValueType && Nullable.GetUnderlyingType(call.Type) is null)
            {
                result = Expression.Coalesce(result, Expression.Default(call.Type));
            }

            return result.Type == call.Type ? result : Expression.Convert(result, call.Type);
        }

        /// <summary>
        /// Checks that <paramref name="key"/> is a property of the current row no two rows share: a key, or the only property of a
        /// unique index.
        /// </summary>
        private bool IsUnique(Expression key)
        {
            return key is MemberExpression { Expression: var instance, Member: var member }
                && instance == row
                && model.FindEntityType(row.Type) is { } entityType
                && entityType.FindProperty(member) is { IsNullable: false } property
                && (entityType.GetKeys().Any(k => k.Properties is [var p] && p == property)
                    || entityType.GetIndexes().Any(i => i.IsUnique && i.Properties is [var p] && p == property));
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
            (Expression Key, ExpressionType Comparison)? ordering = null;
            foreach (var predicate in predicates)
            {
                foreach (var conjunct in Conjuncts(predicate.Body))
                {
                    switch (Correlation(conjunct, predicate.Parameters[0]))
                    {
                        case (var key, ExpressionType.Equal):
                            keys.Add(key);
                            break;
                        case ({ Type.IsValueType: true } key, var comparison) when ordering is null && Nullable.GetUnderlyingType(key.Type) is null:
                            ordering = (key, comparison);
                            break;
                        default:
                            return null;
                    }
                }
            }

            var strict = ordering is (_, ExpressionType.LessThan or ExpressionType.GreaterThan);
            if (strict && call.Method.Name is not (nameof(Queryable.Count) or nameof(Queryable.LongCount)))
            {
                return null;
            }

            List<(LambdaExpression Key, bool Descending)> orderings = ordering is var (orderingKey, direction)
                ? [(Expression.Lambda(orderingKey, row), direction is ExpressionType.GreaterThan or ExpressionType.GreaterThanOrEqual)]
                : [];
            var over = OverClauseBuilder.Build(row, keys, orderings, requireOrderBy: false);

            // The rows strictly before the current one in order are as many as its RANK() - 1.
            return strict
                ? Expression.Convert(Expression.Subtract(Expression.Call(RankMethod, OverClauseBuilder.Functions, over), Expression.Constant(1L)), call.Type)
                : call.Method.Name switch
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
        /// Recognizes <c>t.Key == r.Key</c> or a comparison such as <c>t.Key &lt;= r.Key</c>, in either order, where both sides are
        /// the same expression of their row.
        /// </summary>
        /// <returns>
        /// The key of the current row and the comparison, written as the other row's key compared with the current row's, or
        /// <see langword="null"/> for anything else.
        /// </returns>
        private (Expression Key, ExpressionType Comparison)? Correlation(Expression conjunct, ParameterExpression other)
        {
            if (conjunct is not BinaryExpression comparison
                || comparison.NodeType is not (ExpressionType.Equal or ExpressionType.LessThan or ExpressionType.LessThanOrEqual
                    or ExpressionType.GreaterThan or ExpressionType.GreaterThanOrEqual))
            {
                return null;
            }

            var (otherSide, rowSide, flipped) = (References(comparison.Left, other), References(comparison.Left, row), References(comparison.Right, other), References(comparison.Right, row)) switch
            {
                (true, false, false, true) => (comparison.Left, comparison.Right, false),
                (false, true, true, false) => (comparison.Right, comparison.Left, true),
                _ => (null, null, false),
            };

            return otherSide is null
                || !ExpressionEqualityComparer.Instance.Equals(ReplacingExpressionVisitor.Replace(other, row, otherSide), rowSide)
                ? null
                : (rowSide, (comparison.NodeType, flipped) switch
                {
                    (ExpressionType.LessThan, true) => ExpressionType.GreaterThan,
                    (ExpressionType.LessThanOrEqual, true) => ExpressionType.GreaterThanOrEqual,
                    (ExpressionType.GreaterThan, true) => ExpressionType.LessThan,
                    (ExpressionType.GreaterThanOrEqual, true) => ExpressionType.LessThanOrEqual,
                    (var type, _) => type,
                });
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
