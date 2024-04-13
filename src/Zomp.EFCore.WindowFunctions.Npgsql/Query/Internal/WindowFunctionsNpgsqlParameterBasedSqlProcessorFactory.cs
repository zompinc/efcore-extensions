namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// Factory for producing <see cref="WindowFunctionsNpgsqlParameterBasedSqlProcessor"/> instances.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlParameterBasedSqlProcessorFactory"/> class.
/// </remarks>
/// <param name="dependencies">Relational Parameter Based Sql ProcessorDependencies.</param>
public class WindowFunctionsNpgsqlParameterBasedSqlProcessorFactory(RelationalParameterBasedSqlProcessorDependencies dependencies)
    : NpgsqlParameterBasedSqlProcessorFactory(dependencies)
{
    private readonly RelationalParameterBasedSqlProcessorDependencies dependencies = dependencies;

    /// <inheritdoc/>
    public override RelationalParameterBasedSqlProcessor Create(bool useRelationalNulls)
        => new WindowFunctionsNpgsqlParameterBasedSqlProcessor(dependencies, useRelationalNulls);
}