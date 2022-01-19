namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Factory for producing <see cref="WindowRelationalParameterBasedSqlProcessor"/> instances.
/// </summary>
public class WindowRelationalParameterBasedSqlProcessorFactory : RelationalParameterBasedSqlProcessorFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowRelationalParameterBasedSqlProcessorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Relational Parameter Based Sql ProcessorDependencies.</param>
    public WindowRelationalParameterBasedSqlProcessorFactory(RelationalParameterBasedSqlProcessorDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <inheritdoc/>
    public override RelationalParameterBasedSqlProcessor Create(bool useRelationalNulls)
        => new WindowRelationalParameterBasedSqlProcessor(Dependencies, useRelationalNulls);
}