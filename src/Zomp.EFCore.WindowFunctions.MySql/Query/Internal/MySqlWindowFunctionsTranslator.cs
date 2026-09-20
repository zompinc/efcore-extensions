namespace Zomp.EFCore.WindowFunctions.MySql.Query.Internal;

/// <summary>
/// A SQL translator for window functions in MySQL.
/// </summary>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
public class MySqlWindowFunctionsTranslator(ISqlExpressionFactory sqlExpressionFactory)
    : WindowFunctionsTranslator(sqlExpressionFactory)
{
    /// <inheritdoc/>
    protected override SqlExpression Parse(IReadOnlyList<SqlExpression> arguments, string functionName, Type? resultType)
    {
        // MySQL has no RESPECT NULLS or IGNORE NULLS that does anything, and respecting nulls is how it behaves.
        var retval = base.Parse(WithoutRespectNulls(arguments), functionName, resultType);

        // MySQL answers with its widest type: BIGINT UNSIGNED for ROW_NUMBER and RANK, BIGINT for COUNT, DECIMAL for SUM.
        // The cast gives the result the type mapping of what the method returns, so that it is read as that type.
        return new SqlUnaryExpression(ExpressionType.Convert, retval, retval.Type, null);
    }
}