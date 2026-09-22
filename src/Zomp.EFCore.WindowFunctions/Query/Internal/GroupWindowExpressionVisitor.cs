namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Rewrites GroupBy followed by SelectMany over each group, which EF Core does not translate, to a Select over the rows with
/// window functions partitioned by the key: the index within the group becomes ROW_NUMBER, the group's Count, LongCount, Sum,
/// Min, Max and Average become the aggregate over the partition, and the group's Key becomes the key itself.
/// </summary>
/// <remarks>
/// Only a group read in those ways is rewritten: <c>g</c>, optionally ordered with OrderBy and ThenBy, then projected with
/// Select. Any other use of the group, such as filtering it, leaves the query for EF Core to reject.
/// </remarks>
internal sealed class GroupWindowExpressionVisitor : ExpressionVisitor
{
    private static readonly MethodInfo GroupByMethod = typeof(Queryable).GetMethods()
        .Single(m => m.Name == nameof(Queryable.GroupBy) && m.GetParameters().Length == 2);

    private static readonly MethodInfo SelectManyMethod = typeof(Queryable).GetMethods()
        .Single(m => m.Name == nameof(Queryable.SelectMany) && m.GetParameters().Length == 2 && SelectorParameterCount(m) == 1);

    private static readonly MethodInfo SelectMethod = typeof(Queryable).GetMethods()
        .Single(m => m.Name == nameof(Queryable.Select) && SelectorParameterCount(m) == 1);

    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        var visited = (MethodCallExpression)base.VisitMethodCall(node);

        return visited.Method.IsGenericMethod
            && visited.Method.GetGenericMethodDefinition() == SelectManyMethod
            && visited.Arguments is [MethodCallExpression { Method.IsGenericMethod: true } groupBy, UnaryExpression { Operand: LambdaExpression selector }]
            && groupBy.Method.GetGenericMethodDefinition() == GroupByMethod
            && groupBy.Arguments[1] is UnaryExpression { Operand: LambdaExpression key }
            ? Rewrite(groupBy.Arguments[0], key, selector) ?? visited
            : visited;
    }

    /// <returns>The rewritten query, or <see langword="null"/> when the group is used in a way a window cannot express.</returns>
    private static MethodCallExpression? Rewrite(Expression source, LambdaExpression key, LambdaExpression selector)
    {
        var group = selector.Parameters[0];
        var body = selector.Body;

        LambdaExpression? projection = null;
        if (body is MethodCallExpression { Method: { Name: nameof(Enumerable.Select) } select } selectCall
            && select.DeclaringType == typeof(Enumerable)
            && selectCall.Arguments[1] is LambdaExpression lambda)
        {
            projection = lambda;
            body = selectCall.Arguments[0];
        }

        var orderings = new List<(LambdaExpression Key, bool Descending)>();
        while (body is MethodCallExpression { Method: var method } ordering
            && method.DeclaringType == typeof(Enumerable)
            && method.Name is nameof(Enumerable.OrderBy) or nameof(Enumerable.OrderByDescending)
                or nameof(Enumerable.ThenBy) or nameof(Enumerable.ThenByDescending)
            && ordering.Arguments is [var ordered, LambdaExpression orderingKey])
        {
            orderings.Insert(0, (orderingKey, method.Name.EndsWith("Descending", StringComparison.Ordinal)));
            body = ordered;
        }

        if (body != group)
        {
            return null;
        }

        var element = projection?.Parameters[0] ?? Expression.Parameter(key.Parameters[0].Type, "r");
        var result = projection?.Body ?? element;

        if (projection is { Parameters.Count: 2 })
        {
            // Within a group LINQ keeps the order of the rows GroupBy was given, unless the group is ordered itself.
            if ((orderings.Count > 0 ? orderings : OverClauseBuilder.FindOrderings(source)) is not { } indexOrderings)
            {
                return null;
            }

            var index = OverClauseBuilder.Index(OverClauseBuilder.Build(element, key, indexOrderings, requireOrderBy: true));
            result = ReplacingExpressionVisitor.Replace(projection.Parameters[1], index, result);
        }

        var replacer = new GroupReplacer(group, element, key, OverClauseBuilder.Build(element, key, [], requireOrderBy: false));
        result = replacer.Visit(result);

        return replacer.Unsupported ? null : Expression.Call(
            SelectMethod.MakeGenericMethod(element.Type, result.Type),
            source,
            Expression.Quote(Expression.Lambda(result, element)));
    }

    private static int SelectorParameterCount(MethodInfo method)
        => method.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments().Length - 1;

    /// <summary>
    /// Replaces each use of the group with the window function or key expression for the current row.
    /// </summary>
    private sealed class GroupReplacer(ParameterExpression group, ParameterExpression element, LambdaExpression key, Expression over)
        : ExpressionVisitor
    {
        private static readonly MethodInfo CountMethod = typeof(DbFunctionsExtensions).GetMethods()
            .Single(m => m.Name == nameof(DbFunctionsExtensions.Count) && !m.IsGenericMethod);

        /// <summary>
        /// Gets a value indicating whether the group is used in a way a window cannot express.
        /// </summary>
        public bool Unsupported { get; private set; }

        /// <inheritdoc/>
        protected override Expression VisitMember(MemberExpression node)
            => node.Expression == group && node.Member.Name == nameof(IGrouping<,>.Key)
                ? ReplacingExpressionVisitor.Replace(key.Parameters[0], element, key.Body)
                : base.VisitMember(node);

        /// <inheritdoc/>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType != typeof(Enumerable) || node.Arguments is not [var first, ..] || first != group)
            {
                return base.VisitMethodCall(node);
            }

            var aggregate = node switch
            {
                { Method.Name: nameof(Enumerable.Count), Arguments.Count: 1 } => Expression.Call(CountMethod, OverClauseBuilder.Functions, over),
                { Method.Name: nameof(Enumerable.LongCount), Arguments.Count: 1 }
                    => Expression.Convert(Expression.Call(CountMethod, OverClauseBuilder.Functions, over), typeof(long)),
                { Method.Name: nameof(Enumerable.Sum) or nameof(Enumerable.Min) or nameof(Enumerable.Max) or nameof(Enumerable.Average), Arguments: [_, LambdaExpression value] }
                    => Aggregate(node.Method.Name, ReplacingExpressionVisitor.Replace(value.Parameters[0], element, value.Body), node.Type),
                _ => null,
            };

            if (aggregate is null)
            {
                Unsupported = true;
                return node;
            }

            return aggregate;
        }

        /// <inheritdoc/>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            // Every use of the group this rewrite understands is replaced before its parameter is reached.
            Unsupported |= node == group;
            return node;
        }

        /// <summary>
        /// Finds the window function <paramref name="name"/> taking a value of <paramref name="valueType"/>.
        /// </summary>
        private static MethodInfo? WindowMethod(string name, Type valueType)
        {
            var underlying = Nullable.GetUnderlyingType(valueType);
            foreach (var candidate in typeof(DbFunctionsExtensions).GetMethods())
            {
                if (candidate.Name != name || !candidate.IsGenericMethodDefinition || candidate.GetGenericArguments().Length != 1
                    || candidate.GetParameters() is not [_, { ParameterType: var parameter }, _])
                {
                    continue;
                }

                var takesNullable = parameter.IsGenericType && parameter.GetGenericTypeDefinition() == typeof(Nullable<>);
                if (takesNullable != (underlying is not null))
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
        /// The group's aggregate as a window function over the partition, converted to what LINQ returns.
        /// </summary>
        /// <remarks>
        /// LINQ averages integers as double, where SQL Server's AVG of an integer column is an integer, and sums a group of nulls
        /// to 0, where SUM returns NULL. The databases also disagree on the type of a sum of integers, int on SQL Server and bigint
        /// on PostgreSQL, so integers are summed as bigint, or decimal for a bigint column, and cast to the type LINQ returns.
        /// </remarks>
        private Expression? Aggregate(string name, Expression value, Type resultType)
        {
            var type = Nullable.GetUnderlyingType(value.Type) ?? value.Type;
            var widened = (name, Type.GetTypeCode(type)) switch
            {
                (nameof(Enumerable.Average), not (TypeCode.Decimal or TypeCode.Double)) => typeof(double),
                (nameof(Enumerable.Sum), TypeCode.Byte or TypeCode.SByte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32) => typeof(long),
                (nameof(Enumerable.Sum), TypeCode.Int64) => typeof(decimal),
                _ => null,
            };

            if (widened is not null)
            {
                value = Expression.Convert(value, Nullable.GetUnderlyingType(value.Type) is null ? widened : typeof(Nullable<>).MakeGenericType(widened));
            }

            if (WindowMethod(name == nameof(Enumerable.Average) ? nameof(DbFunctionsExtensions.Avg) : name, value.Type) is not { } method)
            {
                return null;
            }

            Expression call = Expression.Call(method, OverClauseBuilder.Functions, value, over);
            if (name == nameof(Enumerable.Sum) && Nullable.GetUnderlyingType(call.Type) is { } underlying)
            {
                call = Expression.Coalesce(call, Expression.Constant(Activator.CreateInstance(underlying), underlying));
            }

            return call.Type == resultType ? call : Expression.Convert(call, resultType);
        }
    }
}
