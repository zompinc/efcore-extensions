namespace Zomp.EFCore.WindowFunctions.Oracle.Query.Internal;

/// <summary>
/// A SQL translator for window functions in SQLite.
/// </summary>
public class WindowFunctionsOracleTranslator : WindowFunctionsTranslator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsOracleTranslator"/> class.
    /// </summary>
    /// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
    /// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
    public WindowFunctionsOracleTranslator(ISqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource relationalTypeMappingSource)
        : base(sqlExpressionFactory, relationalTypeMappingSource)
    {
    }

    /// <inheritdoc/>
    protected override SqlExpression Parse(IReadOnlyList<SqlExpression> arguments, string functionName)
    {
        var retval = base.Parse(arguments, functionName);

        // Oracle returns decimal even when int32 is expected
        // This is a workaround until a better solution is found
        if (retval.Type != typeof(decimal))
        {
            retval = new SqlUnaryExpression(ExpressionType.Convert, retval, retval.Type, null);
        }

        return retval;
    }
}