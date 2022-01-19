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
            nameof(DbFunctionsExtensions.Min) => MaxMinOver(arguments, "MIN"),
            nameof(DbFunctionsExtensions.Max) => MaxMinOver(arguments, "MAX"),
            nameof(DbFunctionsExtensions.OrderBy) => OrderBy(arguments, true),
            nameof(DbFunctionsExtensions.OrderByDescending) => OrderBy(arguments, false),
            nameof(DbFunctionsExtensions.PartitionBy) => new PartitionByExpression(arguments.Skip(1).First()),
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
    protected virtual SqlExpression MaxMinOver(IReadOnlyList<SqlExpression> arguments, string functionName)
    {
        var expression = arguments[1];
        OrderingSqlExpression? orderingSqlExpression = null;
        PartitionByExpression? partitionBySqlExpression = null;
        foreach (var arg in arguments.Skip(2))
        {
            if (arg is OrderingSqlExpression ose)
            {
                orderingSqlExpression = ose;
            }

            if (arg is PartitionByExpression pbe)
            {
                partitionBySqlExpression = pbe;
            }
        }

        return new WindowFunctionExpression(functionName, sqlExpressionFactory.ApplyDefaultTypeMapping(expression), partitionBySqlExpression?.List, orderingSqlExpression?.List, orderingSqlExpression?.RowOrRangeClause, RelationalTypeMapping.NullMapping);
    }

    private static OrderingSqlExpression GetOrderingSqlExpression(IReadOnlyList<SqlExpression> arguments)
    {
        return arguments is not { Count: > 0 } || arguments[0] is not OrderingSqlExpression orderingSqlExpression
            ? throw new InvalidOperationException($"Must be applied to {nameof(OrderingSqlExpression)}")
            : orderingSqlExpression;
    }

    private static WindowFrame GetWindowFrame(SqlExpression sqlExpression, bool isFollowing)
        => (sqlExpression as SqlConstantExpression) switch
        {
            { } sce when sce.Value is uint value => new BoundedWindowFrame(value, isFollowing),
            _ => throw new InvalidOperationException($"Could not parse {sqlExpression}"),
        };

    private static SqlExpression RowsOrRange(IReadOnlyList<SqlExpression> arguments, bool isRows)
    {
        var orderingSqlExpression = GetOrderingSqlExpression(arguments);

        orderingSqlExpression.RowOrRangeClause = new(isRows, WindowFrame.Unbounded, WindowFrame.CurrentRow);

        return orderingSqlExpression;
    }

    private static SqlExpression From(IReadOnlyList<SqlExpression> arguments, bool isFollowingForBounded)
    {
        var orderingSqlExpression = GetOrderingSqlExpression(arguments);
        var windowFrame = GetWindowFrame(arguments[1], isFollowingForBounded);

        return FromWindowFrame(orderingSqlExpression, windowFrame);
    }

    private static SqlExpression FromWindowFrame(OrderingSqlExpression orderingSqlExpression, WindowFrame windowFrame)
    {
        if (orderingSqlExpression.RowOrRangeClause is null)
        {
            throw new InvalidOperationException("Ensure Rows or Range is called first");
        }

        orderingSqlExpression.RowOrRangeClause = new(orderingSqlExpression.RowOrRangeClause.IsRows, windowFrame);
        return orderingSqlExpression;
    }

    private static SqlExpression To(IReadOnlyList<SqlExpression> arguments, bool isFollowingForBounded)
    {
        var orderingSqlExpression = GetOrderingSqlExpression(arguments);
        var windowFrame = GetWindowFrame(arguments[1], isFollowingForBounded);

        return ToWindowFrame(orderingSqlExpression, windowFrame);
    }

    private static SqlExpression ToWindowFrame(OrderingSqlExpression orderingSqlExpression, WindowFrame windowFrame)
    {
        if (orderingSqlExpression.RowOrRangeClause is null)
        {
            throw new InvalidOperationException("Ensure Rows or Range is called first");
        }

        orderingSqlExpression.RowOrRangeClause = new(orderingSqlExpression.RowOrRangeClause.IsRows, orderingSqlExpression.RowOrRangeClause.Start, windowFrame);
        return orderingSqlExpression;
    }

    private OrderingSqlExpression OrderBy(IReadOnlyList<SqlExpression> arguments, bool ascending)
        => new(new OrderingExpression(sqlExpressionFactory.ApplyDefaultTypeMapping(arguments[1]), ascending));

    private SqlExpression ThenBy(IReadOnlyList<SqlExpression> arguments, bool ascending)
    {
        var arr = arguments.ToList();
        var list = arr[0];
        var chained = sqlExpressionFactory.ApplyDefaultTypeMapping(arr[1]);

        if (list is OrderingSqlExpression orderingSqlExpression)
        {
            orderingSqlExpression.Add(new OrderingExpression(chained, ascending));
        }
        else if (list is PartitionByExpression partitionByExpression)
        {
            partitionByExpression.Add(chained);
        }

        return list;
    }
}