namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// A factory for creating <see cref="WindowQuerySqlGenerator" /> instances.
/// </summary>
public class WindowQuerySqlGeneratorFactory : QuerySqlGeneratorFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowQuerySqlGeneratorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Query Sql Generator Dependencies.</param>
    public WindowQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <inheritdoc/>
    public override QuerySqlGenerator Create()
        => new WindowQuerySqlGenerator(Dependencies);
}