namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Factory for producing <see cref="WindowFunctionsRelationalParameterBasedSqlProcessor"/> instances.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsRelationalParameterBasedSqlProcessorFactory"/> class.
/// </remarks>
/// <param name="dependencies">Relational Parameter Based Sql ProcessorDependencies.</param>
public class WindowFunctionsRelationalParameterBasedSqlProcessorFactory(RelationalParameterBasedSqlProcessorDependencies dependencies) : RelationalParameterBasedSqlProcessorFactory(dependencies)
{
    /// <inheritdoc/>
    public override RelationalParameterBasedSqlProcessor Create(bool useRelationalNulls)
        => new WindowFunctionsRelationalParameterBasedSqlProcessor(Dependencies, useRelationalNulls);
}
