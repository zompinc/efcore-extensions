namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// A SQL translator for window functions.
/// </summary>
public class WindowFunctionsTranslator : IMethodCallTranslator
{
    private readonly ISqlExpressionFactory sqlExpressionFactory;
    private readonly IRelationalTypeMappingSource relationalTypeMappingSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsTranslator"/> class.
    /// </summary>
    /// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
    /// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
    public WindowFunctionsTranslator(ISqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource relationalTypeMappingSource)
    {
        this.sqlExpressionFactory = sqlExpressionFactory;
        this.relationalTypeMappingSource = relationalTypeMappingSource;
    }

    /// <inheritdoc/>
    public SqlExpression? Translate(SqlExpression? instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        => method.Name switch
        {
            nameof(DbFunctionsExtensions.Min) => Over(arguments, "MIN"),
            nameof(DbFunctionsExtensions.Max) => Over(arguments, "MAX"),
            nameof(DbFunctionsExtensions.Sum) => Over(arguments, "SUM"),
            nameof(DbFunctionsExtensions.Avg) => Over(arguments, "AVG"),
            nameof(DbFunctionsExtensions.Count) => Over(arguments, "COUNT"),
            nameof(DbFunctionsExtensions.RowNumber) => Over(arguments, "ROW_NUMBER", 0),
            nameof(DbFunctionsExtensions.Rank) => Over(arguments, "RANK", 0),
            nameof(DbFunctionsExtensions.DenseRank) => Over(arguments, "DENSE_RANK", 0),
            nameof(DbFunctionsExtensions.PercentRank) => Over(arguments, "PERCENT_RANK", 0),

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
    /// <param name="startIndex">0-based index of the first over parameter.</param>
    /// <returns>A SQL translation of the <see cref="MethodCallExpression" />.</returns>
    protected virtual SqlExpression Over(IReadOnlyList<SqlExpression> arguments, string functionName, int startIndex = 1)
    {
        //// For count there needs to be an option to call for
        //// new SqlConstantExpression(Expression.Constant("*"), null)

        var expression = startIndex == 1 ? arguments[startIndex] : null;
        OrderingSqlExpression? orderingSqlExpression = null;
        PartitionByExpression? partitionBySqlExpression = null;

        if (arguments.Count > 1 && arguments[startIndex + 1] is OverExpression over)
        {
            orderingSqlExpression = over.OrderingExpression;
            partitionBySqlExpression = over.PartitionByExpression;
        }

        return new WindowFunctionExpression(functionName, expression is not null ? sqlExpressionFactory.ApplyDefaultTypeMapping(expression) : null, partitionBySqlExpression?.List, orderingSqlExpression?.List, orderingSqlExpression?.RowOrRangeClause, RelationalTypeMapping.NullMapping);
    }

    private static OverExpression GetOrderingSqlExpression(IReadOnlyList<SqlExpression> arguments)
    {
        return arguments is not { Count: > 0 } || arguments[0] is not OverExpression orderingSqlExpression
            ? throw new InvalidOperationException($"Must be applied to {nameof(OverExpression)}")
            : orderingSqlExpression;
    }

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