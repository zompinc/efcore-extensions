namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

internal abstract class ChainedSqlExpression<T> : SqlExpression
    where T : Expression
{
    public ChainedSqlExpression(T first)
        : base(typeof(ChainedSqlExpression<T>), null)
    {
        List = new List<T>(new T[] { first });
    }

    public IReadOnlyList<T> List { get; }

    public void Add(T item) => ((List<T>)List).Add(item);

    protected override void Print(ExpressionPrinter expressionPrinter)
    {
        expressionPrinter.VisitCollection(List);
    }
}