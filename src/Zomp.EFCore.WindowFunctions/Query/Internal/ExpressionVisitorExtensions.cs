namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Extension to visit <see cref="WindowFunctionExpression"/>.
/// </summary>
public static class ExpressionVisitorExtensions
{
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
        expressionVisitor.Visit(windowFunctionExpression.Expression);
        relationalCommandBuilder.Append(") OVER(");
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
}