namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

internal sealed class OrderingSqlExpression(OrderingExpression ordering)
    : ChainedSqlExpression<OrderingExpression>(ordering)
{
#if !EF_CORE_8
    private static ConstructorInfo? quotingConstructor;
#endif

    public RowOrRangeExpression? RowOrRangeClause { get; set; }

#if !EF_CORE_8
    public override Expression Quote()
        => New(quotingConstructor ??= typeof(OrderingSqlExpression).GetConstructor([typeof(OrderingExpression)])!, List[0].Quote());
#endif
}