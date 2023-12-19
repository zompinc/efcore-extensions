namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// A class that processes a SQL tree based on nullability of nodes to apply null semantics in use and optimize it based on parameter values.
/// </summary>
public static class WindowFunctionsSqlNullabilityProcessorHelper
{
    /// <summary>
    /// Visits <see cref="WindowFunctionExpression" /> added by providers and computes its nullability.
    /// </summary>
    /// <param name="windowFunctionExpression">A window function expression to visit.</param>
    /// <param name="visit">Visit callback.</param>
    /// <param name="nullable">A bool value indicating whether the sql expression is nullable.</param>
    /// <returns>An optimized sql expression.</returns>
    public static WindowFunctionExpression VisitWindowFunction(
        WindowFunctionExpression windowFunctionExpression,
        Func<SqlExpression?, SqlExpression?> visit,
        out bool nullable)
    {
        nullable = false;

        SqlExpression[]? arguments = null;
        for (var i = 0; i < windowFunctionExpression.Arguments.Count; i++)
        {
            var visitedArgument = visit(windowFunctionExpression.Arguments[i])!;
            if (visitedArgument != windowFunctionExpression.Arguments[i] && arguments is null)
            {
                arguments = new SqlExpression[windowFunctionExpression.Arguments.Count];

                for (var j = 0; j < i; j++)
                {
                    arguments[j] = windowFunctionExpression.Arguments[j];
                }
            }

            if (arguments is not null)
            {
                arguments[i] = visitedArgument;
            }
        }

        SqlExpression[]? partitions = null;
        for (var i = 0; i < windowFunctionExpression.Partitions.Count; i++)
        {
            var partition = windowFunctionExpression.Partitions[i];
            var visitedPartition = visit(partition);
            if (visitedPartition != windowFunctionExpression.Partitions[i] && partitions is null)
            {
                partitions = new SqlExpression[windowFunctionExpression.Partitions.Count];

                for (var j = 0; j < i; j++)
                {
                    partitions[j] = windowFunctionExpression.Partitions[j];
                }
            }

            if (partitions is not null)
            {
                partitions[i] = visitedPartition!;
            }
        }

        OrderingExpression[]? orderings = null;
        for (var i = 0; i < windowFunctionExpression.Orderings.Count; i++)
        {
            var ordering = windowFunctionExpression.Orderings[i];
            var visitedOrdering = ordering.Update(visit(ordering.Expression)!);
            if (visitedOrdering != windowFunctionExpression.Orderings[i] && orderings is null)
            {
                orderings = new OrderingExpression[windowFunctionExpression.Orderings.Count];

                for (var j = 0; j < i; j++)
                {
                    orderings[j] = windowFunctionExpression.Orderings[j];
                }
            }

            if (orderings is not null)
            {
                orderings[i] = visitedOrdering;
            }
        }

        return arguments is not null || orderings is not null || partitions is not null
            ? windowFunctionExpression.Update(
                arguments ?? windowFunctionExpression.Arguments,
                partitions ?? windowFunctionExpression.Partitions,
                orderings ?? windowFunctionExpression.Orderings)
            : windowFunctionExpression;
    }
}