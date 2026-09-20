namespace Zomp.EFCore.WindowFunctions.Sqlite.Query.Internal;

/// <summary>
/// A factory for creating <see cref="WindowFunctionsSqliteQuerySqlGenerator" /> instances.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsSqliteQuerySqlGeneratorFactory"/> class.
/// </remarks>
/// <param name="dependencies">Query Sql Generator Dependencies.</param>
public class WindowFunctionsSqliteQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies)
    : SqliteQuerySqlGeneratorFactory(dependencies)
{
    private readonly QuerySqlGeneratorDependencies dependencies = dependencies;

    /// <inheritdoc/>
    public override QuerySqlGenerator Create()
        => new WindowFunctionsSqliteQuerySqlGenerator(dependencies);
}
