namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// Factory for generating <see cref="WindowFunctionsSqlServerQuerySqlGenerator"/> instances.
/// </summary>
public class WindowFunctionsSqlServerQuerySqlGeneratorFactory : SqlServerQuerySqlGeneratorFactory
{
#if EF7
    private readonly IRelationalTypeMappingSource typeMappingSource;
#endif

#if EF7
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
#if EF7
    public override QuerySqlGenerator Create()
        => new WindowFunctionsSqlServerQuerySqlGenerator(Dependencies, typeMappingSource);
#else
    public override QuerySqlGenerator Create()
        => new WindowFunctionsSqlServerQuerySqlGenerator(Dependencies);
#endif
}