namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

internal abstract class ChainedSqlExpression<T>(T first) : SqlExpression(typeof(ChainedSqlExpression<T>), null)
    where T : Expression
{
    public IReadOnlyList<T> List { get; } = new List<T>([first]);

    public void Add(T item) => ((List<T>)List).Add(item);

    protected override void Print(ExpressionPrinter expressionPrinter) => expressionPrinter.VisitCollection(List);
}