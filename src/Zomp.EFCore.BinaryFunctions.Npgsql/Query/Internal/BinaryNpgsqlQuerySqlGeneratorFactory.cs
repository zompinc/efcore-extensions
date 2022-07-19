namespace Zomp.EFCore.BinaryFunctions.Npgsql.Query.Internal;

/// <summary>
/// Factory for generating <see cref="BinaryNpgsqlQuerySqlGenerator"/>.
/// </summary>
public class BinaryNpgsqlQuerySqlGeneratorFactory : NpgsqlQuerySqlGeneratorFactory
{
    private readonly QuerySqlGeneratorDependencies dependencies;
    private readonly INpgsqlSingletonOptions npgsqlOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryNpgsqlQuerySqlGeneratorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="npgsqlOptions">Options for Npgsql.</param>
    public BinaryNpgsqlQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies, INpgsqlSingletonOptions npgsqlOptions)
        : base(dependencies, npgsqlOptions)
    {
        this.dependencies = dependencies;
        this.npgsqlOptions = npgsqlOptions;
    }

    /// <inheritdoc/>
    public override QuerySqlGenerator Create()
        => new BinaryNpgsqlQuerySqlGenerator(dependencies, npgsqlOptions.ReverseNullOrderingEnabled, npgsqlOptions.PostgresVersion);
}