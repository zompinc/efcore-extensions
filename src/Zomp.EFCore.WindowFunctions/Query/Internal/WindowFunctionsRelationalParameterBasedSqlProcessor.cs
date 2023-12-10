#if EF_CORE_7 || EF_CORE_6
namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Processes select expression.
/// </summary>
public class WindowFunctionsRelationalParameterBasedSqlProcessor : RelationalParameterBasedSqlProcessor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsRelationalParameterBasedSqlProcessor"/> class.
    /// </summary>
    /// <param name="dependencies">RelationalParameterBasedSqlProcessorDependencies.</param>
    /// <param name="useRelationalNulls">A bool value indicating if relational nulls should be used.</param>
    public WindowFunctionsRelationalParameterBasedSqlProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, bool useRelationalNulls)
        : base(dependencies, useRelationalNulls)
    {
    }

    /// <inheritdoc/>
    protected override Expression ProcessSqlNullability(Expression queryExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        => new WindowFunctionsSqlNullabilityProcessor(Dependencies, UseRelationalNulls).Process(queryExpression, parametersValues, out canCache);
}
#endif