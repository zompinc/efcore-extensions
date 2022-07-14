namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Processes select expression.
/// </summary>
public class WindowRelationalParameterBasedSqlProcessor : RelationalParameterBasedSqlProcessor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowRelationalParameterBasedSqlProcessor"/> class.
    /// </summary>
    /// <param name="dependencies">RelationalParameterBasedSqlProcessorDependencies.</param>
    /// <param name="useRelationalNulls">A bool value indicating if relational nulls should be used.</param>
    public WindowRelationalParameterBasedSqlProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, bool useRelationalNulls)
        : base(dependencies, useRelationalNulls)
    {
    }

    /// <inheritdoc/>
#if NET7_0_OR_GREATER
    protected override Expression ProcessSqlNullability(Expression queryExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        => new WindowFunctionsSqlNullabilityProcessor(Dependencies, UseRelationalNulls).Process(queryExpression, parametersValues, out canCache);
#else
    protected override SelectExpression ProcessSqlNullability(SelectExpression selectExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        => new WindowFunctionsSqlNullabilityProcessor(Dependencies, UseRelationalNulls).Process(selectExpression, parametersValues, out canCache);
#endif
}