namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// A query SQL generator to get <see cref="IRelationalCommand" /> for given <see cref="SelectExpression" />.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowQuerySqlGenerator"/> class.
/// </remarks>
/// <param name="dependencies">Query Sql Generator Dependencies.</param>
public class WindowQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies) : QuerySqlGenerator(dependencies)
{
    /// <inheritdoc/>
    protected override Expression VisitExtension(Expression extensionExpression)
        => extensionExpression switch
        {
            WindowFunctionExpression windowFunctionExpression => this.VisitWindowFunction(windowFunctionExpression),
            _ => base.VisitExtension(extensionExpression),
        };
}