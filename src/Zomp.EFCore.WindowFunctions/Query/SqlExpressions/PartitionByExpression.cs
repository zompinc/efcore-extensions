namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

internal sealed class PartitionByExpression(SqlExpression partition) : ChainedSqlExpression<SqlExpression>(partition)
{
#if !EF_CORE_8
    public override Expression Quote() => throw new NotImplementedException();
#endif
}