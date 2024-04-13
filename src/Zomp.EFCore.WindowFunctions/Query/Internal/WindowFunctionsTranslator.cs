namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// A SQL translator for window functions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsTranslator"/> class.
/// </remarks>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
public class WindowFunctionsTranslator(ISqlExpressionFactory sqlExpressionFactory) : IMethodCallTranslator
{
    private readonly ISqlExpressionFactory sqlExpressionFactory = sqlExpressionFactory;

    /// <inheritdoc/>
    public SqlExpression? Translate(SqlExpression? instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        => method.Name switch
        {
            nameof(DbFunctionsExtensions.Min) => Parse(arguments, "MIN"),
            nameof(DbFunctionsExtensions.Max) => Parse(arguments, "MAX"),
            nameof(DbFunctionsExtensions.Lead) => Parse(arguments, "LEAD"),
            nameof(DbFunctionsExtensions.Lag) => Parse(arguments, "LAG"),
            nameof(DbFunctionsExtensions.Sum) => Parse(arguments, "SUM"),
            nameof(DbFunctionsExtensions.Avg) => Parse(arguments, "AVG"),
            nameof(DbFunctionsExtensions.Count) => Parse(arguments, "COUNT"),
            nameof(DbFunctionsExtensions.RowNumber) => Parse(arguments, "ROW_NUMBER"),
            nameof(DbFunctionsExtensions.Rank) => Parse(arguments, "RANK"),
            nameof(DbFunctionsExtensions.DenseRank) => Parse(arguments, "DENSE_RANK"),
            nameof(DbFunctionsExtensions.PercentRank) => Parse(arguments, "PERCENT_RANK"),

            nameof(DbFunctionsExtensions.OrderBy) => OrderBy(arguments, true),
            nameof(DbFunctionsExtensions.OrderByDescending) => OrderBy(arguments, false),
            nameof(DbFunctionsExtensions.PartitionBy) => PartitionBy(arguments),
            nameof(DbFunctionsExtensions.ThenBy) => ThenBy(arguments, true),
            nameof(DbFunctionsExtensions.ThenByDescending) => ThenBy(arguments, false),

            nameof(DbFunctionsExtensions.Rows) => RowsOrRange(arguments, true),
            nameof(DbFunctionsExtensions.Range) => RowsOrRange(arguments, false),

            nameof(DbFunctionsExtensions.FromPreceding) => From(arguments, false),
            nameof(DbFunctionsExtensions.FromFollowing) => From(arguments, true),
            nameof(DbFunctionsExtensions.FromCurrentRow) => FromWindowFrame(GetOrderingSqlExpression(arguments), WindowFrame.CurrentRow),
            nameof(DbFunctionsExtensions.FromUnbounded) => FromWindowFrame(GetOrderingSqlExpression(arguments), WindowFrame.Unbounded),

            nameof(DbFunctionsExtensions.ToFollowing) => To(arguments, true),
            nameof(DbFunctionsExtensions.ToCurrentRow) => ToWindowFrame(GetOrderingSqlExpression(arguments), WindowFrame.CurrentRow),
            nameof(DbFunctionsExtensions.ToUnbounded) => ToWindowFrame(GetOrderingSqlExpression(arguments), WindowFrame.Unbounded),
            nameof(DbFunctionsExtensions.ToPreceding) => To(arguments, false),

            _ => null,
        };

    /// <summary>
    /// Returns max or min sql expression.
    /// </summary>
    /// <param name="arguments">SQL representations of <see cref="MethodCallExpression.Arguments" />.</param>
    /// <param name="functionName">Function name.</param>
    /// <returns>A SQL translation of the <see cref="MethodCallExpression" />.</returns>
    protected virtual SqlExpression Parse(IReadOnlyList<SqlExpression> arguments, string functionName)
    {
        //// For count there needs to be an option to call for
        //// new SqlConstantExpression(Expression.Constant("*"), null)

        var directArgs = new List<SqlExpression>();

        OverExpression? over = null;
        NullHandling? nullHandling = null;

        for (var i = 1; i < arguments.Count; ++i)
        {
            var argument = arguments[i];

            if ((argument is OverExpression o && (over = o) is { })
                || argument.Type == typeof(OverClause))
            {
                break;
            }

            if (argument is SqlConstantExpression sce && sce.Type == typeof(NullHandling))
            {
                nullHandling = (NullHandling?)sce.Value;
                continue;
            }

            directArgs.Add(sqlExpressionFactory.ApplyDefaultTypeMapping(argument));
        }

        return new WindowFunctionExpression(functionName, directArgs, nullHandling, over?.PartitionByExpression?.List, over?.OrderingExpression?.List, over?.OrderingExpression?.RowOrRangeClause, RelationalTypeMapping.NullMapping);
    }

    private static OverExpression GetOrderingSqlExpression(IReadOnlyList<SqlExpression> arguments) => arguments is not { Count: > 0 } || arguments[0] is not OverExpression orderingSqlExpression
            ? throw new InvalidOperationException($"Must be applied to {nameof(OverExpression)}")
            : orderingSqlExpression;

    private static BoundedWindowFrame GetWindowFrame(SqlExpression sqlExpression, bool isFollowing)
        => (sqlExpression as SqlConstantExpression) switch
        {
            { } sce when sce.Value is uint value => new BoundedWindowFrame(value, isFollowing),
            _ => throw new InvalidOperationException($"Could not parse {sqlExpression}"),
        };

    private static OverExpression RowsOrRange(IReadOnlyList<SqlExpression> arguments, bool isRows)
    {
        var overExpression = GetOrderingSqlExpression(arguments);

        overExpression.OrderingExpression!.RowOrRangeClause = new(isRows, WindowFrame.Unbounded, WindowFrame.CurrentRow);

        return overExpression;
    }

    private static OverExpression From(IReadOnlyList<SqlExpression> arguments, bool isFollowingForBounded)
    {
        var overExpression = GetOrderingSqlExpression(arguments);
        var windowFrame = GetWindowFrame(arguments[1], isFollowingForBounded);

        return FromWindowFrame(overExpression, windowFrame);
    }

    private static OverExpression FromWindowFrame(OverExpression overExpression, WindowFrame windowFrame)
    {
        if (overExpression.OrderingExpression!.RowOrRangeClause is null)
        {
            throw new InvalidOperationException("Ensure Rows or Range is called first");
        }

        overExpression.OrderingExpression!.RowOrRangeClause = new(overExpression.OrderingExpression!.RowOrRangeClause.IsRows, windowFrame);
        return overExpression;
    }

    private static OverExpression To(IReadOnlyList<SqlExpression> arguments, bool isFollowingForBounded)
    {
        var orderingSqlExpression = GetOrderingSqlExpression(arguments);
        var windowFrame = GetWindowFrame(arguments[1], isFollowingForBounded);

        return ToWindowFrame(orderingSqlExpression, windowFrame);
    }

    private static OverExpression ToWindowFrame(OverExpression overExpression, WindowFrame windowFrame)
    {
        if (overExpression.OrderingExpression!.RowOrRangeClause is null)
        {
            throw new InvalidOperationException("Ensure Rows or Range is called first");
        }

        overExpression.OrderingExpression!.RowOrRangeClause = new(overExpression.OrderingExpression!.RowOrRangeClause.IsRows, overExpression.OrderingExpression!.RowOrRangeClause.Start, windowFrame);
        return overExpression;
    }

    private OverExpression OrderBy(IReadOnlyList<SqlExpression> arguments, bool ascending)
    {
        var p = (arguments[0] as OverExpression)?.PartitionByExpression;
        return new(new OrderingSqlExpression(new OrderingExpression(sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]), ascending)), p, false);
    }

    private OverExpression PartitionBy(IReadOnlyList<SqlExpression> arguments)
    {
        var o = (arguments[0] as OverExpression)?.OrderingExpression;
        return new OverExpression(o, new PartitionByExpression(sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1])), true);
    }

    private OverExpression ThenBy(IReadOnlyList<SqlExpression> arguments, bool ascending)
    {
        var arr = arguments.ToList();
        var over = arr[0] as OverExpression ?? throw new InvalidOperationException("Must be over expression");
        var chained = sqlExpressionFactory.ApplyDefaultTypeMapping(arr[1]);

        if (over.IsLatestPartitionBy)
        {
            over.PartitionByExpression!.Add(chained);
        }
        else
        {
            over.OrderingExpression!.Add(new OrderingExpression(chained, ascending));
        }

        return over;
    }
}