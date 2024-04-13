namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// Factory for generating <see cref="WindowFunctionsSqlServerQuerySqlGenerator"/> instances.
/// </summary>
[SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Multiple versions")]
public class WindowFunctionsSqlServerQuerySqlGeneratorFactory : SqlServerQuerySqlGeneratorFactory
{
#if !EF_CORE_7 && !EF_CORE_6
    private readonly ISqlServerSingletonOptions sqlServerSingletonOptions;
#endif
#if !EF_CORE_6
    private readonly IRelationalTypeMappingSource typeMappingSource;
#endif

#if !EF_CORE_7 && !EF_CORE_6
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqlServerQuerySqlGeneratorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="typeMappingSource">Type mapping source.</param>
    /// <param name="sqlServerSingletonOptions">The singleton option.</param>
    public WindowFunctionsSqlServerQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies, IRelationalTypeMappingSource typeMappingSource, ISqlServerSingletonOptions sqlServerSingletonOptions)
        : base(dependencies, typeMappingSource, sqlServerSingletonOptions)
    {
        this.typeMappingSource = typeMappingSource;
        this.sqlServerSingletonOptions = sqlServerSingletonOptions;
    }
#elif !EF_CORE_6
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqlServerQuerySqlGeneratorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="typeMappingSource">Type mapping source.</param>
    public WindowFunctionsSqlServerQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies, IRelationalTypeMappingSource typeMappingSource)
        : base(dependencies, typeMappingSource)
    {
        this.typeMappingSource = typeMappingSource;
    }
#else
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqlServerQuerySqlGeneratorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    public WindowFunctionsSqlServerQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies)
        : base(dependencies)
    {
    }
#endif

    /// <inheritdoc/>
#if !EF_CORE_7 && !EF_CORE_6
    public override QuerySqlGenerator Create()
        => new WindowFunctionsSqlServerQuerySqlGenerator(Dependencies, typeMappingSource, sqlServerSingletonOptions);
#elif !EF_CORE_6
    public override QuerySqlGenerator Create()
        => new WindowFunctionsSqlServerQuerySqlGenerator(Dependencies, typeMappingSource);
#else
    public override QuerySqlGenerator Create()
        => new WindowFunctionsSqlServerQuerySqlGenerator(Dependencies);
#endif
}