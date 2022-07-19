using Microsoft.EntityFrameworkCore.Query;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Zomp.EFCore.BinaryFunctions.Npgsql.Query.Internal;

namespace Zomp.EFCore.Combined.Npgsql.Tests;

/// <summary>
/// Factory for generating <see cref="CombinedNpgsqlQuerySqlGenerator"/>.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Temporary")]
public class CombinedNpgsqlQuerySqlGeneratorFFactory : BinaryNpgsqlQuerySqlGeneratorFactory
{
    private readonly QuerySqlGeneratorDependencies dependencies;
    private readonly INpgsqlSingletonOptions npgsqlSingletonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="CombinedNpgsqlQuerySqlGeneratorFFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="npgsqlSingletonOptions">Options for Npgsql.</param>
    public CombinedNpgsqlQuerySqlGeneratorFFactory(QuerySqlGeneratorDependencies dependencies, INpgsqlSingletonOptions npgsqlSingletonOptions)
        : base(dependencies, npgsqlSingletonOptions)
    {
        this.dependencies = dependencies;
        this.npgsqlSingletonOptions = npgsqlSingletonOptions;
    }

    public override QuerySqlGenerator Create()
        => new CombinedNpgsqlQuerySqlGenerator(dependencies, npgsqlSingletonOptions.ReverseNullOrderingEnabled, npgsqlSingletonOptions.PostgresVersion);
}
