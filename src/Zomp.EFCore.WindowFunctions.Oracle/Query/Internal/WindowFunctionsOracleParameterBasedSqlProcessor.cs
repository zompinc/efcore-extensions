namespace Zomp.EFCore.WindowFunctions.Oracle.Query.Internal;

/// <summary>
/// A class that processes the <see cref="SelectExpression" /> including  window functions.
/// </summary>
public class WindowFunctionsOracleParameterBasedSqlProcessor : OracleParameterBasedSqlProcessor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsOracleParameterBasedSqlProcessor"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="useRelationalNulls">A bool value indicating if relational nulls should be used.</param>
    public WindowFunctionsOracleParameterBasedSqlProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, bool useRelationalNulls)
        : base(dependencies, useRelationalNulls)
    {
    }

    /// <inheritdoc/>
#if !EF_CORE_6

    protected override Expression ProcessSqlNullability(Expression queryExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        => base.ProcessSqlNullability(queryExpression, parametersValues, out canCache);
    /*
    protected override Expression ProcessSqlNullability(Expression selectExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        => new WindowFunctionsOracleSqlNullabilityProcessor(Dependencies, UseRelationalNulls).Process(
            selectExpression, parametersValues, out canCache);
    */
#else
    protected override SelectExpression ProcessSqlNullability(SelectExpression selectExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        => new WindowFunctionsOracleSqlNullabilityProcessor(Dependencies, UseRelationalNulls).Process(
            selectExpression, parametersValues, out canCache);
#endif
}