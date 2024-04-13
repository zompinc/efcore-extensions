namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// Factory for generating <see cref="WindowFunctionsSqlServerParameterBasedSqlProcessor"/> instances.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsSqlServerParameterBasedSqlProcessorFactory"/> class.
/// </remarks>
/// <param name="dependencies">Service dependencies.</param>
public class WindowFunctionsSqlServerParameterBasedSqlProcessorFactory(RelationalParameterBasedSqlProcessorDependencies dependencies)
    : SqlServerParameterBasedSqlProcessorFactory(dependencies)
{
    /// <inheritdoc/>
    public override RelationalParameterBasedSqlProcessor Create(bool useRelationalNulls)
        => new WindowFunctionsSqlServerParameterBasedSqlProcessor(Dependencies, useRelationalNulls);
}