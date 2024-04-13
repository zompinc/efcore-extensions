namespace Zomp.EFCore.WindowFunctions.Sqlite.Query.Internal;

/// <summary>
/// A SQL translator for window functions in SQLite.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SqliteWindowFunctionsTranslator"/> class.
/// </remarks>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
public class SqliteWindowFunctionsTranslator(ISqlExpressionFactory sqlExpressionFactory)
    : WindowFunctionsTranslator(sqlExpressionFactory)
{
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