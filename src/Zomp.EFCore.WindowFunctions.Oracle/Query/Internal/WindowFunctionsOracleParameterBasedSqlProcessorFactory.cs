namespace Zomp.EFCore.WindowFunctions.Oracle.Query.Internal;

/// <summary>
/// Factory for producing <see cref="WindowFunctionsOracleParameterBasedSqlProcessor"/> instances.
/// </summary>
/// <param name="dependencies">Relational Parameter Based Sql ProcessorDependencies.</param>
public class WindowFunctionsOracleParameterBasedSqlProcessorFactory(RelationalParameterBasedSqlProcessorDependencies dependencies)
    : OracleParameterBasedSqlProcessorFactory(dependencies)
{
    /// <inheritdoc/>
    public override RelationalParameterBasedSqlProcessor Create(RelationalParameterBasedSqlProcessorParameters parameters)
        => new WindowFunctionsOracleParameterBasedSqlProcessor(Dependencies, parameters);
}