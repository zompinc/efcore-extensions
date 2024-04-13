namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

internal sealed class OrderingSqlExpression(OrderingExpression ordering) : ChainedSqlExpression<OrderingExpression>(ordering)
{
    public RowOrRangeExpression? RowOrRangeClause { get; set; }
}