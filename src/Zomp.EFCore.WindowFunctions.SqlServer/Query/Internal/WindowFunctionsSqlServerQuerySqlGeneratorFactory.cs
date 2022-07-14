namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// Factory for generating <see cref="WindowFunctionsSqlServerQuerySqlGenerator"/> instances.
/// </summary>
public class WindowFunctionsSqlServerQuerySqlGeneratorFactory : SqlServerQuerySqlGeneratorFactory
{
#if NET7_0_OR_GREATER
    private readonly IRelationalTypeMappingSource typeMappingSource;
#endif

#if NET7_0_OR_GREATER
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
#if NET7_0_OR_GREATER
    public override QuerySqlGenerator Create()
        => new WindowFunctionsSqlServerQuerySqlGenerator(Dependencies, typeMappingSource);
#else
    public override QuerySqlGenerator Create()
        => new WindowFunctionsSqlServerQuerySqlGenerator(Dependencies);
#endif
}