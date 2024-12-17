namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// Query SQL generator for Npgsql which includes window functions operations.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Multiple versions")]
public class WindowFunctionsNpgsqlQuerySqlGenerator : NpgsqlQuerySqlGenerator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlQuerySqlGenerator"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
    /// <param name="reverseNullOrderingEnabled">Null Ordering.</param>
    /// <param name="postgresVersion">Postgres Version.</param>
    public WindowFunctionsNpgsqlQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies, IRelationalTypeMappingSource relationalTypeMappingSource, bool reverseNullOrderingEnabled, Version postgresVersion)
        : base(dependencies, relationalTypeMappingSource, reverseNullOrderingEnabled, postgresVersion)
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