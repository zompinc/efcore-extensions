using Microsoft.EntityFrameworkCore.Query;
#if !EF_CORE_7 && !EF_CORE_6
using Microsoft.EntityFrameworkCore.Storage;
#endif
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
#if !EF_CORE_7 && !EF_CORE_6
    private readonly IRelationalTypeMappingSource relationalTypeMappingSource;
#endif
    private readonly INpgsqlSingletonOptions npgsqlSingletonOptions;

#if !EF_CORE_7 && !EF_CORE_6
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
#else
    /// <summary>
    /// Initializes a new instance of the <see cref="CombinedNpgsqlQuerySqlGeneratorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="npgsqlSingletonOptions">Options for Npgsql.</param>
    public CombinedNpgsqlQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies, INpgsqlSingletonOptions npgsqlSingletonOptions)
        : base(dependencies, npgsqlSingletonOptions)
    {
        this.dependencies = dependencies;
        this.npgsqlSingletonOptions = npgsqlSingletonOptions;
    }
#endif

#if !EF_CORE_7 && !EF_CORE_6
    public override QuerySqlGenerator Create()
        => new CombinedNpgsqlQuerySqlGenerator(dependencies, relationalTypeMappingSource, npgsqlSingletonOptions.ReverseNullOrderingEnabled, npgsqlSingletonOptions.PostgresVersion);
#else
    public override QuerySqlGenerator Create()
        => new CombinedNpgsqlQuerySqlGenerator(dependencies, npgsqlSingletonOptions.ReverseNullOrderingEnabled, npgsqlSingletonOptions.PostgresVersion);
#endif
}
