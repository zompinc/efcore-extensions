namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// Factory for generating <see cref="WindowFunctionsSqlServerQuerySqlGenerator"/> instances.
/// </summary>
public class WindowFunctionsSqlServerQuerySqlGeneratorFactory : SqlServerQuerySqlGeneratorFactory
{
    private readonly IRelationalTypeMappingSource typeMappingSource;

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

    /// <inheritdoc/>
    public override QuerySqlGenerator Create()
        => new WindowFunctionsSqlServerQuerySqlGenerator(Dependencies, typeMappingSource);
}