namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

/// <summary>
/// Contains OrderBy and PartitionBy clauses.
/// </summary>
internal sealed class OverExpression(OrderingSqlExpression? orderingExpression, PartitionByExpression? partitionByExpression, bool isLatestPartitionBy)
    : SqlExpression(typeof(OverExpression), null)
{
    private static ConstructorInfo? quotingConstructor;

    /// <summary>
    /// Gets the partition by clause.
    /// </summary>
    public PartitionByExpression? PartitionByExpression { get; } = partitionByExpression;

    /// <summary>
    /// Gets a value indicating whether the last element in the chain was Partition clause.
    /// </summary>
    public bool IsLatestPartitionBy { get; } = isLatestPartitionBy;

    /// <summary>
    /// Gets the order by clause.
    /// </summary>
    public OrderingSqlExpression? OrderingExpression { get; } = orderingExpression;

    public override Expression Quote()
        => New(quotingConstructor ??= typeof(OverExpression).GetConstructor([typeof(OrderingExpression), typeof(PartitionByExpression), typeof(bool)])!, OrderingExpression?.Quote() ?? Constant(null, typeof(OrderingSqlExpression)), PartitionByExpression?.Quote() ?? Constant(null, typeof(PartitionByExpression)), Constant(IsLatestPartitionBy, typeof(bool)));

    protected override void Print(ExpressionPrinter expressionPrinter) => throw new NotImplementedException();
}
