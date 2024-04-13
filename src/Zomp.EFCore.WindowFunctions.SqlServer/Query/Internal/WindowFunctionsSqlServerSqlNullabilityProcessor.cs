namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// A class that processes a SQL tree based on nullability of nodes to apply null semantics in use and optimize it based on parameter values.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsSqlServerSqlNullabilityProcessor"/> class.
/// </remarks>
/// <param name="dependencies">Relational Parameter Based Sql Processor Dependencies.</param>
/// <param name="useRelationalNulls">A bool value indicating if relational nulls should be used.</param>
public class WindowFunctionsSqlServerSqlNullabilityProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, bool useRelationalNulls)
    : SqlServerSqlNullabilityProcessor(dependencies, useRelationalNulls)
{
    /// <inheritdoc/>
    protected override SqlExpression VisitCustomSqlExpression(SqlExpression sqlExpression, bool allowOptimizedExpansion, out bool nullable)
    {
        var result = sqlExpression switch
        {
            WindowFunctionExpression windowFunctionExpression
                => WindowFunctionsSqlNullabilityProcessorHelper.VisitWindowFunction(windowFunctionExpression, e => Visit(e, out _), out nullable),
            _ => base.VisitCustomSqlExpression(sqlExpression, allowOptimizedExpansion, out nullable),
        };
        return result;
    }
}