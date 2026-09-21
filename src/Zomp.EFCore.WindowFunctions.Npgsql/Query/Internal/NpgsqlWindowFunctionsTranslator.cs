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
        var retval = base.Parse(WithoutRespectNulls(arguments), functionName, resultType);

        // count returns bigint, ntile integer and the statistical functions numeric for integer input, which
        // Npgsql refuses to read as the int, long or double the method returns.
        if ((functionName == "COUNT" && retval.Type != typeof(long)) || functionName is "NTILE" or "STDDEV_SAMP" or "STDDEV_POP" or "VAR_SAMP" or "VAR_POP")
        {
            retval = new SqlUnaryExpression(ExpressionType.Convert, retval, retval.Type, null);
        }

        return retval;
    }
}