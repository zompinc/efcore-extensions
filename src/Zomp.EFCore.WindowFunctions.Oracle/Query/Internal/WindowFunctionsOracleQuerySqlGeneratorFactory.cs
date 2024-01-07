using Oracle.EntityFrameworkCore.Infrastructure.Internal;

namespace Zomp.EFCore.WindowFunctions.Oracle.Query.Internal;

/// <summary>
/// Factory for generating <see cref="WindowFunctionsOracleQuerySqlGenerator"/> instances.
/// </summary>
public class WindowFunctionsOracleQuerySqlGeneratorFactory : OracleQuerySqlGeneratorFactory
{
#if !EF_CORE_7 && !EF_CORE_6
#endif
#if !EF_CORE_6
    private readonly IRelationalTypeMappingSource typeMappingSource;
    private readonly IOracleOptions oracleOptions;
#endif

#if !EF_CORE_7 && !EF_CORE_6
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsOracleQuerySqlGeneratorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="typeMappingSource">Type mapping source.</param>
    /// <param name="oracleOptions">Oracle options.</param>
    public WindowFunctionsOracleQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies, IRelationalTypeMappingSource typeMappingSource, IOracleOptions oracleOptions)
        : base(dependencies, typeMappingSource, oracleOptions)
    {
        this.typeMappingSource = typeMappingSource;
        this.oracleOptions = oracleOptions;
    }
#elif !EF_CORE_6
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsOracleQuerySqlGeneratorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="typeMappingSource">Type mapping source.</param>
    public WindowFunctionsOracleQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies, IRelationalTypeMappingSource typeMappingSource)
        : base(dependencies, typeMappingSource)
    {
        this.typeMappingSource = typeMappingSource;
    }
#else
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsOracleQuerySqlGeneratorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    public WindowFunctionsOracleQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies)
        : base(dependencies)
    {
    }
#endif

    /// <inheritdoc/>
#if !EF_CORE_7 && !EF_CORE_6
    public override QuerySqlGenerator Create()
        => new WindowFunctionsOracleQuerySqlGenerator(Dependencies, typeMappingSource, oracleOptions.OracleSQLCompatibility);
#elif !EF_CORE_6
    public override QuerySqlGenerator Create()
        => new WindowFunctionsOracleQuerySqlGenerator(Dependencies, typeMappingSource);
#else
    public override QuerySqlGenerator Create()
        => new WindowFunctionsOracleQuerySqlGenerator(Dependencies);
#endif
}