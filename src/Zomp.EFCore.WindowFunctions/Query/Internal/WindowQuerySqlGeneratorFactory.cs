namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// A factory for creating <see cref="WindowQuerySqlGenerator" /> instances.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowQuerySqlGeneratorFactory"/> class.
/// </remarks>
/// <param name="dependencies">Query Sql Generator Dependencies.</param>
public class WindowQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies)
    : QuerySqlGeneratorFactory(dependencies)
{
    /// <inheritdoc/>
    public override QuerySqlGenerator Create()
        => new WindowQuerySqlGenerator(Dependencies);
}