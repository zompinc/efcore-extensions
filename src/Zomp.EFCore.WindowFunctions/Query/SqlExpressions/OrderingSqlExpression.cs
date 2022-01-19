namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

internal class OrderingSqlExpression : ChainedSqlExpression<OrderingExpression>
{
    public OrderingSqlExpression(OrderingExpression ordering)
        : base(ordering)
    {
    }

    public RowOrRangeExpression? RowOrRangeClause { get; set; }
}