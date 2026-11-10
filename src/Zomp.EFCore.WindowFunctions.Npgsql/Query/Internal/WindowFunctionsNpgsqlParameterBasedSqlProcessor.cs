namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// A class that processes the <see cref="SelectExpression" /> including  window functions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlParameterBasedSqlProcessor"/> class.
/// </remarks>
public class WindowFunctionsNpgsqlParameterBasedSqlProcessor : NpgsqlParameterBasedSqlProcessor
{
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

    /// <inheritdoc/>
    protected override Expression ProcessSqlNullability(Expression selectExpression, ParametersCacheDecorator parametersDecorator)
        => new WindowFunctionsNpgsqlSqlNullabilityProcessor(Dependencies, Parameters)
            .Process(selectExpression, parametersDecorator);
}
