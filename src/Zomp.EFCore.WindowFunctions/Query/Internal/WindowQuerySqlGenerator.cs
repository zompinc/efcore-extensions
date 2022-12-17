namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// A query SQL generator to get <see cref="IRelationalCommand" /> for given <see cref="SelectExpression" />.
/// </summary>
public class WindowQuerySqlGenerator : QuerySqlGenerator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowQuerySqlGenerator"/> class.
    /// </summary>
    /// <param name="dependencies">Query Sql Generator Dependencies.</param>
    public WindowQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <inheritdoc/>
    protected override Expression VisitExtension(Expression extensionExpression)
        => extensionExpression switch
        {
            WindowFunctionExpression windowFunctionExpression => this.VisitWindowFunction(windowFunctionExpression),
            MethodCallExpression methodCallExpression => this.TranslateCustomMethods(methodCallExpression)!,
            _ => base.VisitExtension(extensionExpression),
        };
}