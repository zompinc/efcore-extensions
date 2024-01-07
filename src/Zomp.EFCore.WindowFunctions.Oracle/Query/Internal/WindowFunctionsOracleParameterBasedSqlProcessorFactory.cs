namespace Zomp.EFCore.WindowFunctions.Oracle.Query.Internal;

/// <summary>
/// Factory for generating <see cref="WindowFunctionsOracleParameterBasedSqlProcessor"/> instances.
/// </summary>
public class WindowFunctionsOracleParameterBasedSqlProcessorFactory : OracleParameterBasedSqlProcessorFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsOracleParameterBasedSqlProcessorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    public WindowFunctionsOracleParameterBasedSqlProcessorFactory(RelationalParameterBasedSqlProcessorDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <inheritdoc/>
    public override RelationalParameterBasedSqlProcessor Create(bool useRelationalNulls)
        => new WindowFunctionsOracleParameterBasedSqlProcessor(Dependencies, useRelationalNulls);
}