namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// Factory for generating <see cref="WindowFunctionsSqlServerQuerySqlGenerator"/> instances.
/// </summary>
public class WindowFunctionsSqlServerQuerySqlGeneratorFactory : SqlServerQuerySqlGeneratorFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqlServerQuerySqlGeneratorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    public WindowFunctionsSqlServerQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <inheritdoc/>
    public override QuerySqlGenerator Create()
        => new WindowFunctionsSqlServerQuerySqlGenerator(Dependencies);
}