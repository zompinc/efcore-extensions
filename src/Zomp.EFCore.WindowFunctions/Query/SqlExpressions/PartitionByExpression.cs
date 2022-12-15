namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

internal sealed class PartitionByExpression : ChainedSqlExpression<SqlExpression>
{
    public PartitionByExpression(SqlExpression partition)
        : base(partition)
    {
    }
}