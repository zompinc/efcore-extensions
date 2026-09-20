namespace Zomp.EFCore.WindowFunctions.Oracle.Query.Internal;

/// <summary>
/// A class that processes a SQL tree based on nullability of nodes to apply null semantics in use and optimize it based on parameter values.
/// </summary>
/// <param name="dependencies">Service dependencies.</param>
/// <param name="parameters">Processor parameters.</param>
public class WindowFunctionsOracleSqlNullabilityProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, RelationalParameterBasedSqlProcessorParameters parameters)
    : SqlNullabilityProcessor(dependencies, parameters)
{
    /// <inheritdoc/>
    protected override SqlExpression VisitCustomSqlExpression(SqlExpression sqlExpression, bool allowOptimizedExpansion, out bool nullable)
        => sqlExpression is WindowFunctionExpression windowFunctionExpression
            ? WindowFunctionsSqlNullabilityProcessorHelper.VisitWindowFunction(windowFunctionExpression, e => Visit(e, out _), out nullable)
            : base.VisitCustomSqlExpression(sqlExpression, allowOptimizedExpansion, out nullable);
}