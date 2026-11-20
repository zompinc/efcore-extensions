namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

internal sealed class PartitionByExpression(SqlExpression partition) : ChainedSqlExpression<SqlExpression>(partition)
{
    public override Expression Quote() => throw new NotImplementedException();
}