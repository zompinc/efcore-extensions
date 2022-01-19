namespace Zomp.EFCore.WindowFunctions.Clauses;

/// <summary>
/// Complete order by clause with rows / range specification.
/// </summary>
public class OrderByClauseWithRowsOrRange : OrderByClause, IRangeCanBeClosed
{
    private OrderByClauseWithRowsOrRange()
    {
    }
}