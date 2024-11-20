namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// A class that processes the <see cref="SelectExpression" /> including  window functions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlParameterBasedSqlProcessor"/> class.
/// </remarks>
public class WindowFunctionsNpgsqlParameterBasedSqlProcessor : NpgsqlParameterBasedSqlProcessor
{
#if !EF_CORE_8
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlParameterBasedSqlProcessor"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="parameters">Processor parameters.</param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "EF Core 8")]
    public WindowFunctionsNpgsqlParameterBasedSqlProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, RelationalParameterBasedSqlProcessorParameters parameters)
        : base(dependencies, parameters)
    {
    }
#else
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlParameterBasedSqlProcessor"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="useRelationalNulls">A bool value indicating if relational nulls should be used.</param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "EF Core 8")]
    public WindowFunctionsNpgsqlParameterBasedSqlProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, bool useRelationalNulls)
        : base(dependencies, useRelationalNulls)
    {
    }
#endif

#if !EF_CORE_8
    /// <inheritdoc/>
    protected override Expression ProcessSqlNullability(Expression selectExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        => new WindowFunctionsNpgsqlSqlNullabilityProcessor(Dependencies, Parameters)
        .Process(selectExpression, parametersValues, out canCache);
#else
    /// <inheritdoc/>
    protected override Expression ProcessSqlNullability(Expression selectExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        => new WindowFunctionsNpgsqlSqlNullabilityProcessor(Dependencies, UseRelationalNulls).Process(selectExpression, parametersValues, out canCache);
#endif

}
