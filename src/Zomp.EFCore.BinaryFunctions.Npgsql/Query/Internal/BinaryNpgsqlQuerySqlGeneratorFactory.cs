namespace Zomp.EFCore.BinaryFunctions.Npgsql.Query.Internal;

/// <summary>
/// Factory for generating <see cref="BinaryNpgsqlQuerySqlGenerator"/>.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Multiple versions")]
public class BinaryNpgsqlQuerySqlGeneratorFactory : NpgsqlQuerySqlGeneratorFactory
{
    private readonly QuerySqlGeneratorDependencies dependencies;
#if !EF_CORE_7 && !EF_CORE_6
    private readonly IRelationalTypeMappingSource relationalTypeMappingSource;
#endif
    private readonly INpgsqlSingletonOptions npgsqlOptions;

#if !EF_CORE_7 && !EF_CORE_6
    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryNpgsqlQuerySqlGeneratorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
    /// <param name="npgsqlOptions">Options for Npgsql.</param>
    public BinaryNpgsqlQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies, IRelationalTypeMappingSource relationalTypeMappingSource, INpgsqlSingletonOptions npgsqlOptions)
        : base(dependencies, relationalTypeMappingSource, npgsqlOptions)
    {
        this.dependencies = dependencies;
        this.relationalTypeMappingSource = relationalTypeMappingSource;
        this.npgsqlOptions = npgsqlOptions;
    }
#else
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
#endif

#if !EF_CORE_7 && !EF_CORE_6
    /// <inheritdoc/>
    public override QuerySqlGenerator Create()
        => new BinaryNpgsqlQuerySqlGenerator(dependencies, relationalTypeMappingSource, npgsqlOptions.ReverseNullOrderingEnabled, npgsqlOptions.PostgresVersion);
#else
    /// <inheritdoc/>
    public override QuerySqlGenerator Create()
        => new BinaryNpgsqlQuerySqlGenerator(dependencies, npgsqlOptions.ReverseNullOrderingEnabled, npgsqlOptions.PostgresVersion);
#endif
}