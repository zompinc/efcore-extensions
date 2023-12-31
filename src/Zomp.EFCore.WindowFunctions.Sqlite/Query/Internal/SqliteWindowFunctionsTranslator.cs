namespace Zomp.EFCore.WindowFunctions.Sqlite.Query.Internal;

/// <summary>
/// A SQL translator for window functions in SQLite.
/// </summary>
public class SqliteWindowFunctionsTranslator : WindowFunctionsTranslator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteWindowFunctionsTranslator"/> class.
    /// </summary>
    /// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
    /// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
    public SqliteWindowFunctionsTranslator(ISqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource relationalTypeMappingSource)
        : base(sqlExpressionFactory, relationalTypeMappingSource)
    {
    }

    /// <inheritdoc/>
    protected override SqlExpression Parse(IReadOnlyList<SqlExpression> arguments, string functionName)
    {
        var retval = base.Parse(arguments, functionName);

        // SQLite returns int64 even when int32 is expected
        // This is a workaround until a better solution is found
        if (retval.Type != typeof(long))
        {
            retval = new SqlUnaryExpression(ExpressionType.Convert, retval, retval.Type, null);
        }

        return retval;
    }
}