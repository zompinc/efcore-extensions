namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

internal class PartitionByExpression : ChainedSqlExpression<SqlExpression>
{
    public PartitionByExpression(SqlExpression partition)
        : base(partition)
    {
    }
}