namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// A class that processes a SQL tree based on nullability of nodes to apply null semantics in use and optimize it based on parameter values.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlSqlNullabilityProcessor"/> class.
/// </remarks>
public class WindowFunctionsNpgsqlSqlNullabilityProcessor : NpgsqlSqlNullabilityProcessor
{
#if !EF_CORE_8
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlSqlNullabilityProcessor"/> class.
    /// </summary>
    /// <param name="dependencies">Relational Parameter Based Sql Processor Dependencies.</param>
    /// <param name="parameters">Processor parameters.</param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "EF Core 8")]
    public WindowFunctionsNpgsqlSqlNullabilityProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, RelationalParameterBasedSqlProcessorParameters parameters)
        : base(dependencies, parameters)
    {
    }
#else
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlSqlNullabilityProcessor"/> class.
    /// </summary>
    /// <param name="dependencies">Relational Parameter Based Sql Processor Dependencies.</param>
    /// <param name="useRelationalNulls">A bool value indicating if relational nulls should be used.</param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "EF Core 8")]
    public WindowFunctionsNpgsqlSqlNullabilityProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, bool useRelationalNulls)
        : base(dependencies, useRelationalNulls)
    {
    }
#endif

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