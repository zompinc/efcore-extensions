namespace Zomp.EFCore.WindowFunctions.MySql.Query.Internal;

/// <summary>
/// Query SQL generator for MySQL which includes window functions operations.
/// </summary>
/// <param name="dependencies">Service dependencies.</param>
/// <param name="typeMappingSource">Instance relational type mapping source.</param>
/// <param name="options">MySQL options.</param>
public class WindowFunctionsMySqlQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies, IRelationalTypeMappingSource typeMappingSource, IMySqlOptions options)
    : MySqlQuerySqlGenerator(dependencies, typeMappingSource, options)
{
    /// <inheritdoc/>
    protected override Expression VisitExtension(Expression extensionExpression)
        => extensionExpression switch
        {
            WindowFunctionExpression windowFunctionExpression => this.VisitWindowFunction(windowFunctionExpression),
            _ => base.VisitExtension(extensionExpression),
        };
}