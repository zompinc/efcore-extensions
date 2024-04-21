namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// A class that processes a SQL tree based on nullability of nodes to apply null semantics in use and optimize it based on parameter values.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsSqlNullabilityProcessor"/> class.
/// </remarks>
/// <param name="dependencies">Relational Parameter Based Sql Processor Dependencies.</param>
/// <param name="useRelationalNulls">A bool value indicating if relational nulls should be used.</param>
public class WindowFunctionsSqlNullabilityProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, bool useRelationalNulls) : SqlNullabilityProcessor(dependencies, useRelationalNulls)
{
    /// <inheritdoc/>
    protected override SqlExpression VisitCustomSqlExpression(SqlExpression sqlExpression, bool allowOptimizedExpansion, out bool nullable)
    {
        var result = sqlExpression switch
        {
            WindowFunctionExpression windowFunctionExpression
                => VisitWindowFunction(windowFunctionExpression, allowOptimizedExpansion, out nullable),
            _ => base.VisitCustomSqlExpression(sqlExpression, allowOptimizedExpansion, out nullable),
        };
        return result;
    }

    /// <summary>
    /// Visits <see cref="WindowFunctionExpression" /> added by providers and computes its nullability.
    /// </summary>
    /// <param name="windowFunctionExpression">A window function expression to visit.</param>
    /// <param name="allowOptimizedExpansion">A bool value indicating if optimized expansion which considers null value as false value is allowed.</param>
    /// <param name="nullable">A bool value indicating whether the sql expression is nullable.</param>
    /// <returns>An optimized sql expression.</returns>
    protected virtual SqlExpression VisitWindowFunction(
        WindowFunctionExpression windowFunctionExpression,
        bool allowOptimizedExpansion,
        out bool nullable)
    {
        nullable = false;
        return windowFunctionExpression;
    }
}
