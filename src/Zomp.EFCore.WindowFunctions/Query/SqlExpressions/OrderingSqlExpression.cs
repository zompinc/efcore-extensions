namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

internal sealed class OrderingSqlExpression : ChainedSqlExpression<OrderingExpression>
{
    public OrderingSqlExpression(OrderingExpression ordering)
        : base(ordering)
    {
    }

    public RowOrRangeExpression? RowOrRangeClause { get; set; }
}