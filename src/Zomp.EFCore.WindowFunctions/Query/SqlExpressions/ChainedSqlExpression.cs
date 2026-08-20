namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

internal abstract class ChainedSqlExpression<T>(T first) : SqlExpression(typeof(ChainedSqlExpression<T>), null)
    where T : Expression
{
    private readonly List<T> list = [first];

    public IReadOnlyList<T> List => list;

    public void Add(T item) => list.Add(item);

    protected override void Print(ExpressionPrinter expressionPrinter) => expressionPrinter.VisitCollection(List);
}