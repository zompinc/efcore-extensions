namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// Factory for producing <see cref="WindowFunctionsNpgsqlParameterBasedSqlProcessor"/> instances.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlParameterBasedSqlProcessorFactory"/> class.
/// </remarks>
public class WindowFunctionsNpgsqlParameterBasedSqlProcessorFactory : NpgsqlParameterBasedSqlProcessorFactory
{
    private readonly RelationalParameterBasedSqlProcessorDependencies dependencies;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlParameterBasedSqlProcessorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Relational Parameter Based Sql ProcessorDependencies.</param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "EF Core 8")]
    public WindowFunctionsNpgsqlParameterBasedSqlProcessorFactory(RelationalParameterBasedSqlProcessorDependencies dependencies)
        : base(dependencies)
    {
        this.dependencies = dependencies;
    }

    /// <inheritdoc/>
    public override RelationalParameterBasedSqlProcessor Create(RelationalParameterBasedSqlProcessorParameters parameters)
        => new WindowFunctionsNpgsqlParameterBasedSqlProcessor(dependencies, parameters);
}