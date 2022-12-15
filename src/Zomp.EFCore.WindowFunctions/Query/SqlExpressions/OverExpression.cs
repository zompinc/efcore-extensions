namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

/// <summary>
/// Contains OrderBy and PartitionBy clauses.
/// </summary>
internal sealed class OverExpression : SqlExpression
{
    public OverExpression(OrderingSqlExpression? orderingExpression, PartitionByExpression? partitionByExpression, bool isLatestPartitionBy)
        : base(typeof(OverExpression), null)
    {
        OrderingExpression = orderingExpression;
        PartitionByExpression = partitionByExpression;
        IsLatestPartitionBy = isLatestPartitionBy;
    }

    /// <summary>
    /// Gets the partition by clause.
    /// </summary>
    public PartitionByExpression? PartitionByExpression { get; }

    /// <summary>
    /// Gets a value indicating whether the last element in the chain was Partition clause.
    /// </summary>
    public bool IsLatestPartitionBy { get; }

    /// <summary>
    /// Gets the order by clause.
    /// </summary>
    public OrderingSqlExpression? OrderingExpression { get; }

    protected override void Print(ExpressionPrinter expressionPrinter)
    {
        throw new NotImplementedException();
    }
}
