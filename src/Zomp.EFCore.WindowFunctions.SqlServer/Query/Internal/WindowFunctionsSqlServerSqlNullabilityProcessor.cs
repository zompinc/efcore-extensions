namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// A class that processes a SQL tree based on nullability of nodes to apply null semantics in use and optimize it based on parameter values.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsSqlServerSqlNullabilityProcessor"/> class.
/// </remarks>
/// <param name="dependencies">Relational Parameter Based Sql Processor Dependencies.</param>
/// <param name="parameters">Relational parameters.</param>
/// <param name="sqlServerSingletonOptions">The singleton option.</param>
public class WindowFunctionsSqlServerSqlNullabilityProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, RelationalParameterBasedSqlProcessorParameters parameters, ISqlServerSingletonOptions sqlServerSingletonOptions)
    : SqlServerSqlNullabilityProcessor(dependencies, parameters, sqlServerSingletonOptions)
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
