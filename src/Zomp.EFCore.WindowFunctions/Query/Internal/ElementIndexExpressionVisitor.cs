namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Rewrites Select and Where with an element index, which EF Core does not translate, to use ROW_NUMBER() - 1 in place of the
/// index. The window is ordered like the rows reaching the operator, by the OrderBy and ThenBy calls before it.
/// </summary>
/// <remarks>
/// Has to run before <see cref="WindowFunctionProjectionDetector"/>, which then pushes the query into a subquery where a filter
/// follows the numbering or a row limit precedes it, so the rows are numbered as LINQ numbers them.
/// </remarks>
internal sealed class ElementIndexExpressionVisitor : ExpressionVisitor
{
    private static readonly MethodInfo SelectMethod = typeof(Queryable).GetMethods()
        .Single(m => m.Name == nameof(Queryable.Select) && SelectorParameterCount(m) == 1);

    private static readonly MethodInfo SelectWithIndexMethod = typeof(Queryable).GetMethods()
        .Single(m => m.Name == nameof(Queryable.Select) && SelectorParameterCount(m) == 2);

    private static readonly MethodInfo WhereMethod = typeof(Queryable).GetMethods()
        .Single(m => m.Name == nameof(Queryable.Where) && SelectorParameterCount(m) == 1);

    private static readonly MethodInfo WhereWithIndexMethod = typeof(Queryable).GetMethods()
        .Single(m => m.Name == nameof(Queryable.Where) && SelectorParameterCount(m) == 2);

    private static readonly MethodInfo RowNumberMethod = typeof(DbFunctionsExtensions).GetMethods()
        .Single(m => m.Name == nameof(DbFunctionsExtensions.RowNumber) && !m.IsGenericMethod);

    private static readonly MethodInfo OrderByMethod = FunctionsMethod(nameof(DbFunctionsExtensions.OrderBy), typeof(OverClause));
    private static readonly MethodInfo OrderByDescendingMethod = FunctionsMethod(nameof(DbFunctionsExtensions.OrderByDescending), typeof(OverClause));
    private static readonly MethodInfo ThenByMethod = FunctionsMethod(nameof(DbFunctionsExtensions.ThenBy), typeof(OrderByClause));
    private static readonly MethodInfo ThenByDescendingMethod = FunctionsMethod(nameof(DbFunctionsExtensions.ThenByDescending), typeof(OrderByClause));

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

    // What EF.Functions in a query is evaluated to before translation.
    private static readonly Expression Functions = Expression.Constant(EF.Functions);

    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        var visited = (MethodCallExpression)base.VisitMethodCall(node);

        if (!visited.Method.IsGenericMethod
            || visited.Arguments is not [var source, UnaryExpression { NodeType: ExpressionType.Quote, Operand: LambdaExpression lambda }])
        {
            return visited;
        }

        var method = visited.Method.GetGenericMethodDefinition();
        return method == SelectWithIndexMethod ? RewriteSelect(source, lambda) ?? visited
            : method == WhereWithIndexMethod ? RewriteWhere(source, lambda) ?? visited
            : visited;
    }

    /// <summary>
    /// Rewrites Where with an index to number the rows with Select with an index, filter them, and project the rows back out.
    /// </summary>
    private static MethodCallExpression? RewriteWhere(Expression source, LambdaExpression predicate)
    {
        var elementType = predicate.Parameters[0].Type;
        var indexedType = typeof(IndexedElement<>).MakeGenericType(elementType);
        var item = indexedType.GetProperty(nameof(IndexedElement<>.Item))!;
        var indexProperty = indexedType.GetProperty(nameof(IndexedElement<>.Index))!;

        var element = Expression.Parameter(elementType, "e");
        var index = Expression.Parameter(typeof(int), "i");
        var numbering = Expression.Lambda(
            Expression.MemberInit(Expression.New(indexedType), Expression.Bind(item, element), Expression.Bind(indexProperty, index)),
            element,
            index);

        if (RewriteSelect(source, numbering) is not { } numbered)
        {
            return null;
        }

        var indexed = Expression.Parameter(indexedType, "w");
        var filter = new ReplacingExpressionVisitor(
                [predicate.Parameters[0], predicate.Parameters[1]],
                [Expression.Property(indexed, item), Expression.Property(indexed, indexProperty)])
            .Visit(predicate.Body);

        var where = Expression.Call(WhereMethod.MakeGenericMethod(indexedType), numbered, Expression.Quote(Expression.Lambda(filter, indexed)));
        return Expression.Call(
            SelectMethod.MakeGenericMethod(indexedType, elementType),
            where,
            Expression.Quote(Expression.Lambda(Expression.Property(indexed, item), indexed)));
    }

    /// <returns>The rewritten Select, or <see langword="null"/> when the rows are ordered by something it cannot follow.</returns>
    private static MethodCallExpression? RewriteSelect(Expression source, LambdaExpression selector)
    {
        (source, selector) = FoldProjections(source, selector);
        var element = selector.Parameters[0];
        if (Over(source, element) is not { } over)
        {
            return null;
        }

        var index = Expression.Convert(
            Expression.Subtract(Expression.Call(RowNumberMethod, Functions, over), Expression.Constant(1L)),
            typeof(int));

        var body = ReplacingExpressionVisitor.Replace(selector.Parameters[1], index, selector.Body);
        var select = SelectMethod.MakeGenericMethod(element.Type, selector.ReturnType);

        return Expression.Call(select, source, Expression.Quote(Expression.Lambda(body, element)));
    }

    /// <summary>
    /// Folds the plain projections between the ordering and <paramref name="selector"/> into the selector, moving Where, Skip and
    /// Take ahead of them as EF Core does, so the OrderBy and ThenBy calls apply to the same element as the selector.
    /// </summary>
    /// <remarks>
    /// A projection with a window function is not folded: moving a filter or row limit ahead of it would change the window.
    /// </remarks>
    private static (Expression Source, LambdaExpression Selector) FoldProjections(Expression source, LambdaExpression selector)
    {
        while (true)
        {
            var operators = new List<MethodCallExpression>();
            var current = source;
            while (current is MethodCallExpression { Method: var method } call
                && method.DeclaringType == typeof(Queryable)
                && (method.Name is nameof(Queryable.Skip) or nameof(Queryable.Take)
                    || (method.Name == nameof(Queryable.Where) && call.Arguments[1] is UnaryExpression { Operand: LambdaExpression { Parameters.Count: 1 } })))
            {
                operators.Add(call);
                current = call.Arguments[0];
            }

            if (current is not MethodCallExpression { Method.IsGenericMethod: true } select
                || select.Method.GetGenericMethodDefinition() != SelectMethod
                || select.Arguments[1] is not UnaryExpression { Operand: LambdaExpression projection }
                || ContainsWindowFunction(projection))
            {
                return (source, selector);
            }

            var element = projection.Parameters[0];
            var folded = select.Arguments[0];
            for (var i = operators.Count - 1; i >= 0; --i)
            {
                var call = operators[i];
                var method = call.Method.GetGenericMethodDefinition().MakeGenericMethod(element.Type);
                folded = Expression.Call(method, [folded, .. call.Arguments.Skip(1).Select(a => a is UnaryExpression { Operand: LambdaExpression predicate }
                    ? Expression.Quote(Expression.Lambda(ReplacingExpressionVisitor.Replace(predicate.Parameters[0], projection.Body, predicate.Body), element))
                    : a)]);
            }

            source = folded;
            selector = Expression.Lambda(
                ReplacingExpressionVisitor.Replace(selector.Parameters[0], projection.Body, selector.Body),
                element,
                selector.Parameters[1]);
        }
    }

    private static bool ContainsWindowFunction(Expression expression)
    {
        var detector = new WindowFunctionDetectorInternal();
        _ = detector.Visit(expression);
        return detector.WindowFunctionsCollection.Count > 0;
    }

    /// <summary>
    /// Builds the over clause from the OrderBy and ThenBy calls that order <paramref name="source"/>.
    /// </summary>
    /// <remarks>
    /// Without them the rows come in no defined order, and ORDER BY a constant numbers them in whatever order the database
    /// reads them. SQL Server requires an ORDER BY in ROW_NUMBER.
    /// </remarks>
    /// <returns>The over clause, or <see langword="null"/> when the rows are ordered by something it cannot follow.</returns>
    private static Expression? Over(Expression source, ParameterExpression element)
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
                if (orderings.Count > 0 || IsOrdered(source))
                {
                    return null;
                }

                break;
            }

            source = call.Arguments[0];
        }

        // What EF.Functions.Over() in a query is evaluated to before translation.
        Expression over = Expression.Constant(OverClause.Instance);
        if (orderings.Count == 0)
        {
            return Expression.Call(OrderByMethod.MakeGenericMethod(typeof(int)), over, Expression.Constant(1));
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

    private static int SelectorParameterCount(MethodInfo select)
        => select.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments().Length - 1;
}
