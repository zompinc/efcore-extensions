namespace Zomp.EFCore.WindowFunctions.Oracle.Query.Internal;

/// <summary>
/// A class that processes the <see cref="SelectExpression" /> after parameter values are known.
/// </summary>
/// <param name="dependencies">Service dependencies.</param>
/// <param name="parameters">Processor parameters.</param>
public class WindowFunctionsOracleParameterBasedSqlProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, RelationalParameterBasedSqlProcessorParameters parameters)
    : OracleParameterBasedSqlProcessor(dependencies, parameters)
{
    /// <inheritdoc/>
    protected override Expression ProcessSqlNullability(Expression queryExpression, ParametersCacheDecorator Decorator)
        => new WindowFunctionsOracleSqlNullabilityProcessor(Dependencies, Parameters).Process(queryExpression, Decorator);
}