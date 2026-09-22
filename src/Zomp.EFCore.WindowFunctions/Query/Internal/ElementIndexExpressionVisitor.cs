namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Rewrites operators EF Core does not translate but a window function over the ordered rows can express: Select and Where with
/// an element index, which use ROW_NUMBER() - 1 in place of the index, DistinctBy, which keeps the first row of each key, and
/// TakeWhile and SkipWhile, which count the rows failing the predicate so far. The window is ordered like the rows reaching the
/// operator, by the OrderBy and ThenBy calls before it.
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

    private static readonly MethodInfo DistinctByMethod = typeof(Queryable).GetMethods()
        .Single(m => m.Name == nameof(Queryable.DistinctBy) && m.GetParameters().Length == 2);

    private static readonly MethodInfo TakeWhileMethod = typeof(Queryable).GetMethods()
        .Single(m => m.Name == nameof(Queryable.TakeWhile) && SelectorParameterCount(m) == 1);

    private static readonly MethodInfo SkipWhileMethod = typeof(Queryable).GetMethods()
        .Single(m => m.Name == nameof(Queryable.SkipWhile) && SelectorParameterCount(m) == 1);

    private static readonly MethodInfo SumOfIntMethod = typeof(DbFunctionsExtensions).GetMethods()
        .Single(m => m.Name == nameof(DbFunctionsExtensions.Sum) && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1
            && m.GetParameters()[1].ParameterType.IsGenericParameter)
        .MakeGenericMethod(typeof(int));

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
            : method == DistinctByMethod ? RewriteDistinctBy(source, lambda) ?? visited
            : method == TakeWhileMethod ? RewriteWhile(source, lambda, take: true) ?? visited
            : method == SkipWhileMethod ? RewriteWhile(source, lambda, take: false) ?? visited
            : visited;
    }

    /// <summary>
    /// Rewrites DistinctBy to number the rows of each key, keep the first and project the rows back out.
    /// </summary>
    /// <remarks>
    /// PARTITION BY puts null keys together, as DistinctBy does. An anonymous key is partitioned by each of its members.
    /// </remarks>
    private static MethodCallExpression? RewriteDistinctBy(Expression source, LambdaExpression key)
        => RewriteNumbered(source, key.Parameters[0].Type, key, (_, index) => Expression.Equal(index, Expression.Constant(0)));

    /// <summary>
    /// Rewrites TakeWhile and SkipWhile to count, for each row, the rows up to it that fail the predicate: TakeWhile keeps the
    /// rows before the first failure, where the count is still 0, and SkipWhile the rest.
    /// </summary>
    /// <remarks>
    /// The predicate is evaluated as C# evaluates it, so a comparison with NULL fails. Without an ordering there is no first row
    /// to fail, and the query is left for EF Core to reject.
    /// </remarks>
    private static MethodCallExpression? RewriteWhile(Expression source, LambdaExpression predicate, bool take)
        => RewriteNumbered(
            source,
            predicate.Parameters[0].Type,
            predicate,
            (_, failures) => take ? Expression.Equal(failures, Expression.Constant(0)) : Expression.GreaterThan(failures, Expression.Constant(0)),
            RunningFailures);

    /// <summary>
    /// Builds SUM(CASE WHEN predicate THEN 0 ELSE 1 END) over the rows from the first to the current one.
    /// </summary>
    private static Expression? RunningFailures(
        ParameterExpression element,
        LambdaExpression? predicate,
        IReadOnlyList<(LambdaExpression Key, bool Descending)> orderings)
    {
        if (orderings.Count == 0)
        {
            return null;
        }

        var failed = Expression.Condition(
            ReplacingExpressionVisitor.Replace(predicate!.Parameters[0], element, predicate.Body),
            Expression.Constant(0),
            Expression.Constant(1));
        var over = OverClauseBuilder.RunningFrame(OverClauseBuilder.Build(element, partition: null, orderings, requireOrderBy: true));

        return Expression.Convert(Expression.Call(SumOfIntMethod, OverClauseBuilder.Functions, failed, over), typeof(int));
    }

    /// <summary>
    /// The row's index within its partition, ROW_NUMBER() - 1, with <paramref name="partition"/> as the partition key if there
    /// is one.
    /// </summary>
    private static Expression Index(
        ParameterExpression element,
        LambdaExpression? partition,
        IReadOnlyList<(LambdaExpression Key, bool Descending)> orderings)
        => OverClauseBuilder.Index(OverClauseBuilder.Build(element, partition, orderings, requireOrderBy: true));

    /// <summary>
    /// Computes a window value for each row of <paramref name="source"/>, ROW_NUMBER() - 1 unless <paramref name="window"/>
    /// builds another, keeps the rows passing <paramref name="condition"/> on the row and its value, and projects the rows back
    /// out. <paramref name="rowLambda"/>, the partition key or predicate over a row, is handed to <paramref name="window"/>.
    /// </summary>
    private static MethodCallExpression? RewriteNumbered(
        Expression source,
        Type elementType,
        LambdaExpression? rowLambda,
        Func<Expression, Expression, Expression> condition,
        Func<ParameterExpression, LambdaExpression?, IReadOnlyList<(LambdaExpression Key, bool Descending)>, Expression?>? window = null)
    {
        var indexedType = typeof(IndexedElement<>).MakeGenericType(elementType);
        var item = indexedType.GetProperty(nameof(IndexedElement<>.Item))!;
        var indexProperty = indexedType.GetProperty(nameof(IndexedElement<>.Index))!;

        var element = Expression.Parameter(elementType, "e");
        var index = Expression.Parameter(typeof(int), "i");
        var numbering = Expression.Lambda(
            Expression.MemberInit(Expression.New(indexedType), Expression.Bind(item, element), Expression.Bind(indexProperty, index)),
            element,
            index);

        if (RewriteSelect(source, numbering, rowLambda, window) is not { } numbered)
        {
            return null;
        }

        var indexed = Expression.Parameter(indexedType, "w");
        var itemAccess = Expression.Property(indexed, item);
        var indexAccess = Expression.Property(indexed, indexProperty);
        var where = Expression.Call(
            WhereMethod.MakeGenericMethod(indexedType),
            numbered,
            Expression.Quote(Expression.Lambda(condition(itemAccess, indexAccess), indexed)));
        return Expression.Call(
            SelectMethod.MakeGenericMethod(indexedType, elementType),
            where,
            Expression.Quote(Expression.Lambda(itemAccess, indexed)));
    }

    /// <summary>
    /// Rewrites Where with an index to number the rows with Select with an index, filter them, and project the rows back out.
    /// </summary>
    private static MethodCallExpression? RewriteWhere(Expression source, LambdaExpression predicate)
        => RewriteNumbered(
            source,
            predicate.Parameters[0].Type,
            rowLambda: null,
            (item, index) => new ReplacingExpressionVisitor([predicate.Parameters[0], predicate.Parameters[1]], [item, index]).Visit(predicate.Body));

    /// <returns>
    /// The rewritten Select, or <see langword="null"/> when the rows are ordered by something it cannot follow or
    /// <paramref name="window"/> cannot build its value.
    /// </returns>
    private static MethodCallExpression? RewriteSelect(
        Expression source,
        LambdaExpression selector,
        LambdaExpression? rowLambda = null,
        Func<ParameterExpression, LambdaExpression?, IReadOnlyList<(LambdaExpression Key, bool Descending)>, Expression?>? window = null)
    {
        (source, selector, rowLambda) = FoldProjections(source, selector, rowLambda);
        var element = selector.Parameters[0];
        if (OverClauseBuilder.FindOrderings(source) is not { } orderings
            || (window ?? Index)(element, rowLambda, orderings) is not { } index)
        {
            return null;
        }

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
    private static (Expression Source, LambdaExpression Selector, LambdaExpression? RowLambda) FoldProjections(
        Expression source,
        LambdaExpression selector,
        LambdaExpression? rowLambda)
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
                return (source, selector, rowLambda);
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
            rowLambda = rowLambda is null
                ? null
                : Expression.Lambda(ReplacingExpressionVisitor.Replace(rowLambda.Parameters[0], projection.Body, rowLambda.Body), element);
        }
    }

    private static bool ContainsWindowFunction(Expression expression)
    {
        var detector = new WindowFunctionDetectorInternal();
        _ = detector.Visit(expression);
        return detector.WindowFunctionsCollection.Count > 0;
    }

    private static int SelectorParameterCount(MethodInfo select)
        => select.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments().Length - 1;
}
