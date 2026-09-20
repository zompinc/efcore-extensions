namespace Zomp.EFCore.WindowFunctions.MySql.Query.Internal;

/// <summary>
/// A class that processes the <see cref="SelectExpression" /> after parameter values are known.
/// </summary>
/// <param name="dependencies">Service dependencies.</param>
/// <param name="parameters">Processor parameters.</param>
/// <param name="options">MySQL options.</param>
public class WindowFunctionsMySqlParameterBasedSqlProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, RelationalParameterBasedSqlProcessorParameters parameters, IMySqlOptions options)
    : MySqlParameterBasedSqlProcessor(dependencies, parameters, options)
{
    /// <inheritdoc/>
    protected override Expression ProcessSqlNullability(Expression queryExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        => new WindowFunctionsMySqlSqlNullabilityProcessor(Dependencies, Parameters).Process(queryExpression, parametersValues, out canCache);
}