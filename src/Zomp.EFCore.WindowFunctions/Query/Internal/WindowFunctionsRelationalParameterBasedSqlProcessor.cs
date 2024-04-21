namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Processes select expression.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsRelationalParameterBasedSqlProcessor"/> class.
/// </remarks>
/// <param name="dependencies">RelationalParameterBasedSqlProcessorDependencies.</param>
/// <param name="useRelationalNulls">A bool value indicating if relational nulls should be used.</param>
public class WindowFunctionsRelationalParameterBasedSqlProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, bool useRelationalNulls) : RelationalParameterBasedSqlProcessor(dependencies, useRelationalNulls)
{
    /// <inheritdoc/>
    protected override Expression ProcessSqlNullability(Expression queryExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        => new WindowFunctionsSqlNullabilityProcessor(Dependencies, UseRelationalNulls).Process(queryExpression, parametersValues, out canCache);
}
