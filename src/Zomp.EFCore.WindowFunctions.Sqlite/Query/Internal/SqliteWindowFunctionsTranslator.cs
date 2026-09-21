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
    private static readonly Dictionary<string, string> StatisticalFunctions = new()
    {
        ["STDDEV_SAMP"] = nameof(DbFunctionsExtensions.StandardDeviationSample),
        ["STDDEV_POP"] = nameof(DbFunctionsExtensions.StandardDeviationPopulation),
        ["VAR_SAMP"] = nameof(DbFunctionsExtensions.VarianceSample),
        ["VAR_POP"] = nameof(DbFunctionsExtensions.VariancePopulation),
    };

    /// <inheritdoc/>
    protected override SqlExpression Parse(IReadOnlyList<SqlExpression> arguments, string functionName, Type? resultType)
    {
        if (StatisticalFunctions.TryGetValue(functionName, out var methodName))
        {
            throw new InvalidOperationException(
                $"SQLite has no {methodName} function. Call UseWindowFunctions(approximateStandardDeviationAndVariance: true) "
                + "to compute it from AVG, SUM and COUNT, which loses precision when the values are large and close together.");
        }

        var retval = base.Parse(WithoutRespectNulls(arguments), functionName, resultType);

        // SQLite returns int64 even when int32 is expected
        // This is a workaround until a better solution is found
        if (retval.Type != typeof(long))
        {
            retval = new SqlUnaryExpression(ExpressionType.Convert, retval, retval.Type, null);
        }

        return retval;
    }
}