namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// A SQL translator for window functions in PostgreSQL.
/// </summary>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
public class NpgsqlWindowFunctionsTranslator(ISqlExpressionFactory sqlExpressionFactory)
    : WindowFunctionsTranslator(sqlExpressionFactory)
{
    /// <inheritdoc/>
    protected override SqlExpression Parse(IReadOnlyList<SqlExpression> arguments, string functionName, Type? resultType)
    {
        var retval = base.Parse(arguments, functionName, resultType);

        // count returns bigint in PostgreSQL, which Npgsql refuses to read as an int.
        if (functionName == "COUNT" && retval.Type != typeof(long))
        {
            retval = new SqlUnaryExpression(ExpressionType.Convert, retval, retval.Type, null);
        }

        return retval;
    }
}