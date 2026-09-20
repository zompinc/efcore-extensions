namespace Zomp.EFCore.WindowFunctions.MySql.Query.Internal;

/// <summary>
/// Factory for producing <see cref="WindowFunctionsMySqlParameterBasedSqlProcessor"/> instances.
/// </summary>
/// <param name="dependencies">Service dependencies.</param>
/// <param name="options">MySQL options.</param>
public class WindowFunctionsMySqlParameterBasedSqlProcessorFactory(RelationalParameterBasedSqlProcessorDependencies dependencies, IMySqlOptions options)
    : IRelationalParameterBasedSqlProcessorFactory
{
    /// <inheritdoc/>
    public RelationalParameterBasedSqlProcessor Create(RelationalParameterBasedSqlProcessorParameters parameters)
        => new WindowFunctionsMySqlParameterBasedSqlProcessor(dependencies, parameters, options);
}