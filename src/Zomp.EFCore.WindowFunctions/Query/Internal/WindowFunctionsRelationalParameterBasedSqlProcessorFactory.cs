namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Factory for producing <see cref="WindowFunctionsRelationalParameterBasedSqlProcessor"/> instances.
/// </summary>
public class WindowFunctionsRelationalParameterBasedSqlProcessorFactory : RelationalParameterBasedSqlProcessorFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsRelationalParameterBasedSqlProcessorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Relational Parameter Based Sql ProcessorDependencies.</param>
    public WindowFunctionsRelationalParameterBasedSqlProcessorFactory(RelationalParameterBasedSqlProcessorDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <inheritdoc/>
    public override RelationalParameterBasedSqlProcessor Create(bool useRelationalNulls)
        => new WindowFunctionsRelationalParameterBasedSqlProcessor(Dependencies, useRelationalNulls);
}
