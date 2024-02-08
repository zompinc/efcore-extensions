namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Extension to visit <see cref="WindowFunctionExpression"/>.
/// </summary>
public static class ExpressionVisitorExtensions
{
    /// <summary>
    /// Replaces parameter.
    /// </summary>
    /// <param name="expression">Expression containing parameter.</param>
    /// <param name="source">Parameter to replace.</param>
    /// <param name="target">Expression to replace with.</param>
    /// <returns>New expression.</returns>
    public static Expression ReplaceParameter(this Expression expression, ParameterExpression source, Expression target)
        => new ParameterReplacer(source, target).Visit(expression);

    /// <summary>
    /// Visits the window function expression.
    /// </summary>
    /// <param name="expressionVisitor">Instance of the <see cref="ExpressionVisitor"/>.</param>
    /// <param name="windowFunctionExpression">Window function expression to visit.</param>
    /// <returns>The original expression.</returns>
    public static Expression VisitWindowFunction(this ExpressionVisitor expressionVisitor, WindowFunctionExpression windowFunctionExpression)
    {
        // Fixme: need a way to avoid reflection of private fields.
        var fi = typeof(QuerySqlGenerator).GetField("_relationalCommandBuilder", BindingFlags.NonPublic | BindingFlags.Instance);
        IRelationalCommandBuilder relationalCommandBuilder = (IRelationalCommandBuilder)fi!.GetValue(expressionVisitor)!;
        relationalCommandBuilder.Append($"{windowFunctionExpression.Function}(");

        if (windowFunctionExpression.Arguments.Count == 0
            && windowFunctionExpression.Function.Equals(nameof(DbFunctionsExtensions.Count), StringComparison.OrdinalIgnoreCase))
        {
            relationalCommandBuilder.Append($"*");
        }
        else
        {
            GenerateList(relationalCommandBuilder, windowFunctionExpression.Arguments, e => expressionVisitor.Visit(e));
        }

        relationalCommandBuilder.Append(") ");

        if (windowFunctionExpression.NullHandling is { } nullHandling)
        {
            relationalCommandBuilder.Append(nullHandling == NullHandling.RespectNulls
                ? "RESPECT NULLS " : "IGNORE NULLS ");
        }

        relationalCommandBuilder.Append("OVER(");
        if (windowFunctionExpression.Partitions.Any())
        {
            relationalCommandBuilder.Append("PARTITION BY ");
            GenerateList(relationalCommandBuilder, windowFunctionExpression.Partitions, e => expressionVisitor.Visit(e));
            relationalCommandBuilder.Append(" ");
        }

        if (windowFunctionExpression.Orderings.Any())
        {
            relationalCommandBuilder.Append("ORDER BY ");
            GenerateList(relationalCommandBuilder, windowFunctionExpression.Orderings, e => expressionVisitor.Visit(e));

            ProcessRowOrRange(windowFunctionExpression, relationalCommandBuilder);
        }

        relationalCommandBuilder.Append(")");

        return windowFunctionExpression;
    }

    private static void ProcessRowOrRange(WindowFunctionExpression windowFunctionExpression, IRelationalCommandBuilder relationalCommandBuilder)
    {
        if (windowFunctionExpression.RowOrRange is null)
        {
            return;
        }

        relationalCommandBuilder.Append(" ");

        relationalCommandBuilder.Append(windowFunctionExpression.RowOrRange.IsRows ? "ROWS " : "RANGE ");

        if (windowFunctionExpression.RowOrRange.End is { } following)
        {
            relationalCommandBuilder.Append("BETWEEN ");
            ProcessWindowFrame(relationalCommandBuilder, windowFunctionExpression.RowOrRange.Start, false);
            relationalCommandBuilder.Append(" AND ");
            ProcessWindowFrame(relationalCommandBuilder, windowFunctionExpression.RowOrRange.End, true);
            return;
        }

        ProcessWindowFrame(relationalCommandBuilder, windowFunctionExpression.RowOrRange.Start, false);
    }

    private static void ProcessWindowFrame(IRelationalCommandBuilder relationalCommandBuilder, WindowFrame windowFrame, bool isStart)
    {
        relationalCommandBuilder.Append(windowFrame.ToString()!);

        if (windowFrame.IsDirectional)
        {
            relationalCommandBuilder.Append(" ");
            bool isFollowing = windowFrame is BoundedWindowFrame bwf ? bwf.IsFollowing : isStart;
            relationalCommandBuilder.Append(isFollowing ? "FOLLOWING" : "PRECEDING");
        }
    }

    private static void GenerateList<T>(
    IRelationalCommandBuilder relationalCommandBuilder,
    IReadOnlyList<T> items,
    Action<T> generationAction,
    Action<IRelationalCommandBuilder>? joinAction = null)
    {
        joinAction ??= isb => isb.Append(", ");

        for (var i = 0; i < items.Count; i++)
        {
            if (i > 0)
            {
                joinAction(relationalCommandBuilder);
            }

            generationAction(items[i]);
        }
    }

    internal sealed class ParameterReplacer(ParameterExpression source, Expression target) : ExpressionVisitor
    {
        private readonly ParameterExpression source = source;
        private readonly Expression target = target;

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == source ? target : base.VisitParameter(node);
        }
    }
}