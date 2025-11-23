namespace Zomp.EFCore.WindowFunctions.Sqlite.Query.Internal;

/// <summary>
/// A class that processes the <see cref="SelectExpression" /> including  window functions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsSqliteParameterBasedSqlProcessor"/> class.
/// </remarks>
public class WindowFunctionsSqliteParameterBasedSqlProcessor : SqliteParameterBasedSqlProcessor
{
#if !EF_CORE_8
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqliteParameterBasedSqlProcessor"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="parameters">Processor parameters.</param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "EF Core 8")]
    public WindowFunctionsSqliteParameterBasedSqlProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, RelationalParameterBasedSqlProcessorParameters parameters)
        : base(dependencies, parameters)
    {
    }
#else
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqliteParameterBasedSqlProcessor"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="useRelationalNulls">A bool value indicating if relational nulls should be used.</param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "EF Core 8")]
    public WindowFunctionsSqliteParameterBasedSqlProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, bool useRelationalNulls)
        : base(dependencies, useRelationalNulls)
    {
    }
#endif

#if !EF_CORE_8 && !EF_CORE_9
    /// <inheritdoc/>
    protected override Expression ProcessSqlNullability(Expression queryExpression, ParametersCacheDecorator parametersDecorator)
        => new WindowFunctionsSqliteSqlNullabilityProcessor(Dependencies, Parameters)
            .Process(queryExpression, parametersDecorator);
#elif !EF_CORE_8 && EF_CORE_9
    /// <inheritdoc/>
    protected override Expression ProcessSqlNullability(Expression queryExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        => new WindowFunctionsSqliteSqlNullabilityProcessor(Dependencies, Parameters)
            .Process(queryExpression, parametersValues, out canCache);
#else
    /// <inheritdoc/>
    protected override Expression ProcessSqlNullability(Expression queryExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        => new WindowFunctionsSqliteSqlNullabilityProcessor(Dependencies, UseRelationalNulls).Process(queryExpression, parametersValues, out canCache);
#endif
}
