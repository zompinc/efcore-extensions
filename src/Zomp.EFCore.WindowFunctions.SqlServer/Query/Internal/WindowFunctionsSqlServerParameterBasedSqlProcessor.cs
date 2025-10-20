namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// A class that processes the <see cref="SelectExpression" /> including  window functions.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Multiple versions")]
public class WindowFunctionsSqlServerParameterBasedSqlProcessor : SqlServerParameterBasedSqlProcessor
{
#if !EF_CORE_8 && !EF_CORE_9
    private readonly ISqlServerSingletonOptions sqlServerSingletonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqlServerParameterBasedSqlProcessor"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="parameters">Processor parameters.</param>
    /// <param name="sqlServerSingletonOptions">The singleton option.</param>
    public WindowFunctionsSqlServerParameterBasedSqlProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, RelationalParameterBasedSqlProcessorParameters parameters, ISqlServerSingletonOptions sqlServerSingletonOptions)
        : base(dependencies, parameters, sqlServerSingletonOptions)
    {
        this.sqlServerSingletonOptions = sqlServerSingletonOptions;
    }
#elif !EF_CORE_8 && EF_CORE_9
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqlServerParameterBasedSqlProcessor"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="parameters">Processor parameters.</param>
    public WindowFunctionsSqlServerParameterBasedSqlProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, RelationalParameterBasedSqlProcessorParameters parameters)
        : base(dependencies, parameters)
    {
    }
#else
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqlServerParameterBasedSqlProcessor"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="useRelationalNulls">A bool value indicating if relational nulls should be used.</param>
    public WindowFunctionsSqlServerParameterBasedSqlProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, bool useRelationalNulls)
        : base(dependencies, useRelationalNulls)
    {
    }
#endif

#if !EF_CORE_8 && !EF_CORE_9
    /// <inheritdoc/>
    protected override Expression ProcessSqlNullability(Expression selectExpression, ParametersCacheDecorator Decorator)
        => new WindowFunctionsSqlServerSqlNullabilityProcessor(Dependencies, Parameters, sqlServerSingletonOptions).Process(
            selectExpression, Decorator);
#elif !EF_CORE_8 && EF_CORE_9
    /// <inheritdoc/>
    protected override Expression ProcessSqlNullability(Expression selectExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        => new WindowFunctionsSqlServerSqlNullabilityProcessor(Dependencies, Parameters).Process(
            selectExpression, parametersValues, out canCache);
#else
    /// <inheritdoc/>
    protected override Expression ProcessSqlNullability(Expression selectExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        => new WindowFunctionsSqlServerSqlNullabilityProcessor(Dependencies, UseRelationalNulls).Process(
            selectExpression, parametersValues, out canCache);
#endif
}