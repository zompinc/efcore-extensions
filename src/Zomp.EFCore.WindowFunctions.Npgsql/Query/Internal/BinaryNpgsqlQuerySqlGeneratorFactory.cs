namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// Factory for generating <see cref="WindowFunctionsNpgsqlQuerySqlGenerator"/>.
/// </summary>
public class WindowFunctionsNpgsqlQuerySqlGeneratorFactory : NpgsqlQuerySqlGeneratorFactory
{
    private readonly QuerySqlGeneratorDependencies dependencies;
    private readonly INpgsqlSingletonOptions npgsqlOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlQuerySqlGeneratorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="npgsqlOptions">Options for Npgsql.</param>
    public WindowFunctionsNpgsqlQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies, INpgsqlSingletonOptions npgsqlOptions)
        : base(dependencies, npgsqlOptions)
    {
        this.dependencies = dependencies;
        this.npgsqlOptions = npgsqlOptions;
    }

    /// <inheritdoc/>
    public override QuerySqlGenerator Create()
        => new WindowFunctionsNpgsqlQuerySqlGenerator(dependencies, npgsqlOptions.ReverseNullOrderingEnabled, npgsqlOptions.PostgresVersion);
}