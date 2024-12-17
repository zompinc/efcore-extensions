namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// A query SQL generator for window functions to get <see cref="IRelationalCommand" /> for given <see cref="SelectExpression" />.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Multiple versions")]
public class WindowFunctionsSqlServerQuerySqlGenerator : SqlServerQuerySqlGenerator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqlServerQuerySqlGenerator"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="typeMappingSource">Type mapping source.</param>
    /// <param name="sqlServerSingletonOptions">The singleton option.</param>
    public WindowFunctionsSqlServerQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies, IRelationalTypeMappingSource typeMappingSource, ISqlServerSingletonOptions sqlServerSingletonOptions)
        : base(dependencies, typeMappingSource, sqlServerSingletonOptions)
    {
    }

    /// <inheritdoc/>
    protected override Expression VisitExtension(Expression extensionExpression)
        => extensionExpression switch
        {
            WindowFunctionExpression windowFunctionExpression => this.VisitWindowFunction(windowFunctionExpression),
            _ => base.VisitExtension(extensionExpression),
        };
}