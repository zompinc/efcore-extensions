namespace Zomp.EFCore.WindowFunctions.Sqlite.Query.Internal;

/// <summary>
/// A query SQL generator for window functions to get <see cref="IRelationalCommand" /> for given <see cref="SelectExpression" />.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Multiple versions")]
public class WindowFunctionsSqliteQuerySqlGenerator : SqliteQuerySqlGenerator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqliteQuerySqlGenerator"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    public WindowFunctionsSqliteQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies)
        : base(dependencies)
    {
    }

    /// <inheritdoc/>
    [SuppressMessage("Naming", "CA1725:Parameter names should match base declaration", Justification = "EF Core 11 renamed the base parameter; one name serves both")]
    protected override Expression VisitExtension(Expression extensionExpression)
        => extensionExpression switch
        {
            WindowFunctionExpression windowFunctionExpression => this.VisitWindowFunction(windowFunctionExpression),
            _ => base.VisitExtension(extensionExpression),
        };
}
