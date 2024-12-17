using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Zomp.EFCore.BinaryFunctions.Npgsql.Query.Internal;

namespace Zomp.EFCore.Combined.Npgsql.Tests;

/// <summary>
/// Factory for generating <see cref="CombinedNpgsqlQuerySqlGenerator"/>.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Temporary")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Multiple versions")]
public class CombinedNpgsqlQuerySqlGeneratorFactory : BinaryNpgsqlQuerySqlGeneratorFactory
{
    private readonly QuerySqlGeneratorDependencies dependencies;
    private readonly IRelationalTypeMappingSource relationalTypeMappingSource;
    private readonly INpgsqlSingletonOptions npgsqlSingletonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="CombinedNpgsqlQuerySqlGeneratorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
    /// <param name="npgsqlSingletonOptions">Options for Npgsql.</param>
    public CombinedNpgsqlQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies, IRelationalTypeMappingSource relationalTypeMappingSource, INpgsqlSingletonOptions npgsqlSingletonOptions)
        : base(dependencies, relationalTypeMappingSource, npgsqlSingletonOptions)
    {
        this.dependencies = dependencies;
        this.relationalTypeMappingSource = relationalTypeMappingSource;
        this.npgsqlSingletonOptions = npgsqlSingletonOptions;
    }

    public override QuerySqlGenerator Create()
        => new CombinedNpgsqlQuerySqlGenerator(dependencies, relationalTypeMappingSource, npgsqlSingletonOptions.ReverseNullOrderingEnabled, npgsqlSingletonOptions.PostgresVersion);
}
