namespace Zomp.EFCore.WindowFunctions.Oracle.Query.Internal;

/// <summary>
/// Factory for generating <see cref="WindowFunctionsOracleParameterBasedSqlProcessor"/> instances.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsOracleParameterBasedSqlProcessorFactory"/> class.
/// </remarks>
/// <param name="dependencies">Service dependencies.</param>
public class WindowFunctionsOracleParameterBasedSqlProcessorFactory(RelationalParameterBasedSqlProcessorDependencies dependencies) : OracleParameterBasedSqlProcessorFactory(dependencies)
{
    /// <inheritdoc/>
    public override RelationalParameterBasedSqlProcessor Create(bool useRelationalNulls)
        => new WindowFunctionsOracleParameterBasedSqlProcessor(Dependencies, useRelationalNulls);
}