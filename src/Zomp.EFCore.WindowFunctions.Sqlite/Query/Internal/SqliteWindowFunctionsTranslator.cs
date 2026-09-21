namespace Zomp.EFCore.WindowFunctions.Sqlite.Query.Internal;

/// <summary>
/// A SQL translator for window functions in SQLite.
/// </summary>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
/// <param name="approximateStandardDeviationAndVariance">Whether to compute standard deviation and variance from AVG, SUM and COUNT.</param>
public class SqliteWindowFunctionsTranslator(ISqlExpressionFactory sqlExpressionFactory, bool approximateStandardDeviationAndVariance)
    : WindowFunctionsTranslator(sqlExpressionFactory)
{
    private static readonly bool[] OneArgumentPropagatesNullability = [true];
    private static readonly bool[] TwoArgumentsPropagateNullability = [true, true];

    private static readonly Dictionary<string, string> StatisticalFunctions = new()
    {
        ["STDDEV_SAMP"] = nameof(DbFunctionsExtensions.StandardDeviationSample),
        ["STDDEV_POP"] = nameof(DbFunctionsExtensions.StandardDeviationPopulation),
        ["VAR_SAMP"] = nameof(DbFunctionsExtensions.VarianceSample),
        ["VAR_POP"] = nameof(DbFunctionsExtensions.VariancePopulation),
    };

    private readonly ISqlExpressionFactory sqlExpressionFactory = sqlExpressionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteWindowFunctionsTranslator"/> class that throws for standard deviation and variance.
    /// </summary>
    /// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
    public SqliteWindowFunctionsTranslator(ISqlExpressionFactory sqlExpressionFactory)
        : this(sqlExpressionFactory, approximateStandardDeviationAndVariance: false)
    {
    }

    /// <inheritdoc/>
    protected override SqlExpression Parse(IReadOnlyList<SqlExpression> arguments, string functionName, Type? resultType)
    {
        SqlExpression retval;
        if (StatisticalFunctions.TryGetValue(functionName, out var methodName))
        {
            if (!approximateStandardDeviationAndVariance)
            {
                throw new InvalidOperationException(
                    $"SQLite has no {methodName} function. Call UseWindowFunctions(approximateStandardDeviationAndVariance: true) "
                    + "to compute it from AVG, SUM and COUNT, which loses precision when the values are large and close together.");
            }

            retval = Approximate(arguments, functionName);
        }
        else
        {
            retval = base.Parse(WithoutRespectNulls(arguments), functionName, resultType);
        }

        // SQLite returns int64 even when int32 is expected
        // This is a workaround until a better solution is found
        if (retval.Type != typeof(long))
        {
            retval = new SqlUnaryExpression(ExpressionType.Convert, retval, retval.Type, null);
        }

        return retval;
    }

    /// <summary>
    /// Computes a standard deviation or variance from aggregates SQLite has, over the window the call was given.
    /// </summary>
    /// <remarks>
    /// Population variance is AVG(x*x) - AVG(x)^2 and sample variance (SUM(x*x) - SUM(x)^2 / COUNT(x)) / (COUNT(x) - 1).
    /// Both subtract two large numbers when the values are large and close together, so rounding can leave a small
    /// negative variance; it is clamped to zero before the square root. COUNT(x) - 1 is 0 for a single value, and SQLite
    /// returns NULL when dividing by zero, as the other databases do for a sample of one.
    /// </remarks>
    private SqlExpression Approximate(IReadOnlyList<SqlExpression> arguments, string functionName)
    {
        // REAL arithmetic: an integer column would otherwise be squared and divided as integers.
        var value = sqlExpressionFactory.Convert(arguments[1], typeof(double));
        var square = sqlExpressionFactory.Multiply(value, value);

        SqlExpression Over(string function, SqlExpression argument)
            => base.Parse([arguments[0], argument, .. arguments.Skip(2)], function, typeof(double));

        SqlExpression variance;
        if (functionName is "VAR_POP" or "STDDEV_POP")
        {
            var average = Over("AVG", value);
            variance = sqlExpressionFactory.Subtract(Over("AVG", square), sqlExpressionFactory.Multiply(average, average));
        }
        else
        {
            var sum = Over("SUM", value);
            var count = Over("COUNT", value);
            var sumOfSquares = sqlExpressionFactory.Subtract(Over("SUM", square), sqlExpressionFactory.Divide(sqlExpressionFactory.Multiply(sum, sum), count));
            variance = sqlExpressionFactory.Divide(sumOfSquares, sqlExpressionFactory.Subtract(count, sqlExpressionFactory.Constant(1.0)));
        }

        variance = sqlExpressionFactory.Function("max", [variance, sqlExpressionFactory.Constant(0.0)], nullable: true, TwoArgumentsPropagateNullability, typeof(double));

        return functionName.StartsWith("STDDEV", StringComparison.Ordinal)
            ? sqlExpressionFactory.Function("sqrt", [variance], nullable: true, OneArgumentPropagatesNullability, typeof(double))
            : variance;
    }
}