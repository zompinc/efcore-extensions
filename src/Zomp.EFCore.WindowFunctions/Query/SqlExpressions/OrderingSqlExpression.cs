namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

internal sealed class OrderingSqlExpression(OrderingExpression ordering)
    : ChainedSqlExpression<OrderingExpression>(ordering)
{
    private static ConstructorInfo? quotingConstructor;

    public RowOrRangeExpression? RowOrRangeClause { get; set; }

    public override Expression Quote()
        => New(quotingConstructor ??= typeof(OrderingSqlExpression).GetConstructor([typeof(OrderingExpression)])!, List[0].Quote());
}