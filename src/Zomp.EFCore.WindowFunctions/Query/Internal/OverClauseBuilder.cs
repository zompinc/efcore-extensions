namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Builds the calls a query would contain for EF.Functions.Over() and its clauses, for rewrites that turn LINQ operators into
/// window functions.
/// </summary>
internal static class OverClauseBuilder
{
    // What EF.Functions in a query is evaluated to before translation.
    internal static readonly Expression Functions = Expression.Constant(EF.Functions);

    private static readonly MethodInfo RowNumberMethod = typeof(DbFunctionsExtensions).GetMethods()
        .Single(m => m.Name == nameof(DbFunctionsExtensions.RowNumber) && !m.IsGenericMethod);

    private static readonly MethodInfo OrderByMethod = FunctionsMethod(nameof(DbFunctionsExtensions.OrderBy), typeof(OverClause));
    private static readonly MethodInfo OrderByDescendingMethod = FunctionsMethod(nameof(DbFunctionsExtensions.OrderByDescending), typeof(OverClause));
    private static readonly MethodInfo ThenByMethod = FunctionsMethod(nameof(DbFunctionsExtensions.ThenBy), typeof(OrderByClause));
    private static readonly MethodInfo ThenByDescendingMethod = FunctionsMethod(nameof(DbFunctionsExtensions.ThenByDescending), typeof(OrderByClause));
    private static readonly MethodInfo PartitionByMethod = FunctionsMethod(nameof(DbFunctionsExtensions.PartitionBy), typeof(OverClause));
    private static readonly MethodInfo ThenPartitionByMethod = FunctionsMethod(nameof(DbFunctionsExtensions.ThenBy), typeof(PartitionByClause));
    private static readonly MethodInfo RowsMethod = FunctionsMethod(nameof(DbFunctionsExtensions.Rows), typeof(OrderByClause));
    private static readonly MethodInfo FromUnboundedMethod = FunctionsMethod(nameof(DbFunctionsExtensions.FromUnbounded), typeof(RowsOrRangeClause));
    private static readonly MethodInfo ToCurrentRowMethod = FunctionsMethod(nameof(DbFunctionsExtensions.ToCurrentRow), typeof(OrderByClauseWithRowsOrRange));

    /// <summary>
    /// Operators that keep the order of the rows they are given.
    /// </summary>
    private static readonly FrozenSet<string> OrderPreservingOperators =
    [
        nameof(DbFunctionsExtensions.AsSubQuery),
        nameof(Queryable.Skip),
        nameof(Queryable.Take),
        nameof(Queryable.Where),
    ];

    /// <summary>
    /// Finds the OrderBy and ThenBy calls that order <paramref name="source"/>.
    /// </summary>
    /// <returns>
    /// The keys, empty when the rows come in no defined order, or <see langword="null"/> when they are ordered by something that
    /// cannot be followed, such as an OrderBy behind a projection or with a comparer.
    /// </returns>
    internal static List<(LambdaExpression Key, bool Descending)>? FindOrderings(Expression source)
    {
        var orderings = new List<(LambdaExpression Key, bool Descending)>();
        while (source is MethodCallExpression { Method: var method } call
            && (method.DeclaringType == typeof(Queryable) || method.DeclaringType == typeof(DbFunctionsExtensions)))
        {
            if (method.Name is nameof(Queryable.OrderBy) or nameof(Queryable.OrderByDescending)
                or nameof(Queryable.ThenBy) or nameof(Queryable.ThenByDescending))
            {
                if (call.Arguments is not [_, UnaryExpression { Operand: LambdaExpression key }])
                {
                    // An OrderBy with a comparer has no SQL ordering to copy.
                    return null;
                }

                orderings.Insert(0, (key, method.Name.EndsWith("Descending", StringComparison.Ordinal)));
                if (method.Name.StartsWith(nameof(Queryable.OrderBy), StringComparison.Ordinal))
                {
                    break;
                }
            }
            else if (!OrderPreservingOperators.Contains(method.Name))
            {
                return orderings.Count > 0 || IsOrdered(source) ? null : orderings;
            }

            source = call.Arguments[0];
        }

        return orderings;
    }

    /// <summary>
    /// Builds an over clause for <paramref name="element"/>, partitioned by the members of <paramref name="partition"/> and
    /// ordered by <paramref name="orderings"/>.
    /// </summary>
    /// <param name="element">The row the keys are taken from.</param>
    /// <param name="partition">The partition key, if any. An anonymous key is partitioned by each of its members.</param>
    /// <param name="orderings">The ordering keys.</param>
    /// <param name="requireOrderBy">
    /// Whether to order by a constant when there are no ordering keys. ROW_NUMBER needs an ORDER BY on SQL Server, while an
    /// aggregate must have none to cover the whole partition.
    /// </param>
    internal static Expression Build(
        ParameterExpression element,
        LambdaExpression? partition,
        IReadOnlyList<(LambdaExpression Key, bool Descending)> orderings,
        bool requireOrderBy)
    {
        if (partition is null)
        {
            return Build(element, [], orderings, requireOrderBy);
        }

        var key = ReplacingExpressionVisitor.Replace(partition.Parameters[0], element, partition.Body);
        return Build(element, key is NewExpression { Arguments.Count: > 0 } anonymous ? anonymous.Arguments : [key], orderings, requireOrderBy);
    }

    /// <summary>
    /// Builds an over clause for <paramref name="element"/>, partitioned by <paramref name="partitionKeys"/>, expressions of the
    /// element, and ordered by <paramref name="orderings"/>.
    /// </summary>
    /// <param name="element">The row the ordering keys are taken from.</param>
    /// <param name="partitionKeys">The partition keys.</param>
    /// <param name="orderings">The ordering keys.</param>
    /// <param name="requireOrderBy">Whether to order by a constant when there are no ordering keys.</param>
    internal static Expression Build(
        ParameterExpression element,
        IReadOnlyList<Expression> partitionKeys,
        IReadOnlyList<(LambdaExpression Key, bool Descending)> orderings,
        bool requireOrderBy)
    {
        // What EF.Functions.Over() in a query is evaluated to before translation.
        Expression over = Expression.Constant(OverClause.Instance);
        for (var i = 0; i < partitionKeys.Count; ++i)
        {
            var method = i == 0 ? PartitionByMethod : ThenPartitionByMethod;
            over = Expression.Call(method.MakeGenericMethod(partitionKeys[i].Type), over, partitionKeys[i]);
        }

        if (orderings.Count == 0)
        {
            return requireOrderBy ? Expression.Call(OrderByMethod.MakeGenericMethod(typeof(int)), over, Expression.Constant(1)) : over;
        }

        for (var i = 0; i < orderings.Count; ++i)
        {
            var (key, descending) = orderings[i];
            var method = (i == 0, descending) switch
            {
                (true, false) => OrderByMethod,
                (true, true) => OrderByDescendingMethod,
                (false, false) => ThenByMethod,
                (false, true) => ThenByDescendingMethod,
            };

            var keyBody = ReplacingExpressionVisitor.Replace(key.Parameters[0], element, key.Body);
            over = Expression.Call(method.MakeGenericMethod(key.ReturnType), over, keyBody);
        }

        return over;
    }

    /// <summary>
    /// Builds ROW_NUMBER() - 1 over <paramref name="over"/>, the zero based index LINQ gives.
    /// </summary>
    internal static Expression Index(Expression over)
        => Expression.Convert(
            Expression.Subtract(Expression.Call(RowNumberMethod, Functions, over), Expression.Constant(1L)),
            typeof(int));

    /// <summary>
    /// Limits an ordered over clause to the rows from the start of the partition to the current row, one row at a time even
    /// when the ordering has ties.
    /// </summary>
    internal static Expression RunningFrame(Expression orderedOver)
        => Expression.Call(ToCurrentRowMethod, Expression.Call(FromUnboundedMethod, Expression.Call(RowsMethod, orderedOver)));

    private static bool IsOrdered(Expression source)
    {
        while (source is MethodCallExpression { Arguments.Count: > 0 } call && typeof(IQueryable).IsAssignableFrom(call.Arguments[0].Type))
        {
            if (call.Method.DeclaringType == typeof(Queryable) && call.Method.Name is nameof(Queryable.OrderBy) or nameof(Queryable.OrderByDescending))
            {
                return true;
            }

            source = call.Arguments[0];
        }

        return false;
    }

    private static MethodInfo FunctionsMethod(string name, Type firstParameter)
        => typeof(DbFunctionsExtensions).GetMethods()
            .Single(m => m.Name == name && m.GetParameters()[0].ParameterType == firstParameter);
}
