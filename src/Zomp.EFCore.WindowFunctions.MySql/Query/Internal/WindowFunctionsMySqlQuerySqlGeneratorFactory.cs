namespace Zomp.EFCore.WindowFunctions.MySql.Query.Internal;

/// <summary>
/// Factory for generating <see cref="WindowFunctionsMySqlQuerySqlGenerator"/> instances.
/// </summary>
/// <param name="dependencies">Service dependencies.</param>
/// <param name="typeMappingSource">Instance relational type mapping source.</param>
/// <param name="options">MySQL options.</param>
public class WindowFunctionsMySqlQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies, IRelationalTypeMappingSource typeMappingSource, IMySqlOptions options)
    : IQuerySqlGeneratorFactory
{
    /// <inheritdoc/>
    public QuerySqlGenerator Create() => new WindowFunctionsMySqlQuerySqlGenerator(dependencies, typeMappingSource, options);
}