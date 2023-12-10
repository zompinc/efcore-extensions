namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// Factory for producing <see cref="WindowFunctionsNpgsqlParameterBasedSqlProcessor"/> instances.
/// </summary>
public class WindowFunctionsNpgsqlParameterBasedSqlProcessorFactory : NpgsqlParameterBasedSqlProcessorFactory
{
    private readonly RelationalParameterBasedSqlProcessorDependencies dependencies;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlParameterBasedSqlProcessorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Relational Parameter Based Sql ProcessorDependencies.</param>
    public WindowFunctionsNpgsqlParameterBasedSqlProcessorFactory(RelationalParameterBasedSqlProcessorDependencies dependencies)
        : base(dependencies)
    {
        this.dependencies = dependencies;
    }

    /// <inheritdoc/>
    public override RelationalParameterBasedSqlProcessor Create(bool useRelationalNulls)
        => new WindowFunctionsNpgsqlParameterBasedSqlProcessor(dependencies, useRelationalNulls);
}