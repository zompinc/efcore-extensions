namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// Factory for generating <see cref="WindowFunctionsNpgsqlQuerySqlGenerator"/>.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Multiple versions")]
public class WindowFunctionsNpgsqlQuerySqlGeneratorFactory : NpgsqlQuerySqlGeneratorFactory
{
    private readonly QuerySqlGeneratorDependencies dependencies;
#if !EF_CORE_7 && !EF_CORE_6
    private readonly IRelationalTypeMappingSource relationalTypeMappingSource;
#endif
    private readonly INpgsqlSingletonOptions npgsqlOptions;

#if !EF_CORE_7 && !EF_CORE_6
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlQuerySqlGeneratorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
    /// <param name="npgsqlOptions">Options for Npgsql.</param>
    public WindowFunctionsNpgsqlQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies, IRelationalTypeMappingSource relationalTypeMappingSource, INpgsqlSingletonOptions npgsqlOptions)
        : base(dependencies, relationalTypeMappingSource, npgsqlOptions)
    {
        this.dependencies = dependencies;
        this.relationalTypeMappingSource = relationalTypeMappingSource;
        this.npgsqlOptions = npgsqlOptions;
    }
#else
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
#endif

#if !EF_CORE_7 && !EF_CORE_6
    /// <inheritdoc/>
    public override QuerySqlGenerator Create()
        => new WindowFunctionsNpgsqlQuerySqlGenerator(dependencies, relationalTypeMappingSource, npgsqlOptions.ReverseNullOrderingEnabled, npgsqlOptions.PostgresVersion);
#else
    /// <inheritdoc/>
    public override QuerySqlGenerator Create()
        => new WindowFunctionsNpgsqlQuerySqlGenerator(dependencies, npgsqlOptions.ReverseNullOrderingEnabled, npgsqlOptions.PostgresVersion);
#endif
}
