namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

/// <summary>
/// An expression that represents a Window Function in a SQL tree.
/// </summary>
public class WindowFunctionExpression : SqlExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionExpression"/> class.
    /// </summary>
    /// <param name="function">Function (MIN, MAX).</param>
    /// <param name="expression">The expression of the Window function.</param>
    /// <param name="partitions">A list expressions to partition by.</param>
    /// <param name="orderings">A list of ordering expressions to order by.</param>
    /// <param name="rowOrRange">Row or range clause.</param>
    /// <param name="typeMapping">The <see cref="RelationalTypeMapping" /> associated with the expression.</param>
    public WindowFunctionExpression(
        string function,
        SqlExpression expression,
        IReadOnlyList<SqlExpression>? partitions,
        IReadOnlyList<OrderingExpression>? orderings,
        RowOrRangeExpression? rowOrRange,
        RelationalTypeMapping? typeMapping)
        : base((expression ?? throw new ArgumentNullException(nameof(expression), "Can't be null")).Type, typeMapping)
    {
        Expression = expression;
        Partitions = partitions ?? Array.Empty<SqlExpression>();
        Orderings = orderings ?? Array.Empty<OrderingExpression>();
        Function = function;
        RowOrRange = rowOrRange;
    }

    /// <summary>
    /// Gets the expression of the window function.
    /// </summary>
    public virtual SqlExpression Expression { get; }

    /// <summary>
    /// Gets the list of expressions used in partitioning.
    /// </summary>
    public virtual IReadOnlyList<SqlExpression> Partitions { get; }

    /// <summary>
    /// Gets list of ordering expressions used to order inside the given partition.
    /// </summary>
    public virtual IReadOnlyList<OrderingExpression> Orderings { get; }

    /// <summary>
    /// Gets the function name.
    /// </summary>
    /// <remarks>
    /// Possible values: Max / Min...
    /// </remarks>
    public string Function { get; }

    /// <summary>
    /// Gets the Row or Range clause.
    /// </summary>
    public RowOrRangeExpression? RowOrRange { get; }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj != null
            && (ReferenceEquals(this, obj)
                || (obj is WindowFunctionExpression windowFunctionsExpression
                && Equals(windowFunctionsExpression)));

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = default(HashCode);
        hash.Add(base.GetHashCode());
        foreach (var partition in Partitions)
        {
            hash.Add(partition);
        }

        foreach (var ordering in Orderings)
        {
            hash.Add(ordering);
        }

        hash.Add(Expression);
        hash.Add(Function);
        hash.Add(RowOrRange);

        return hash.ToHashCode();
    }

    /// <inheritdoc />
    protected override Expression VisitChildren(ExpressionVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var changed = false;
        var partitions = new List<SqlExpression>();
        foreach (var partition in Partitions)
        {
            var newPartition = (SqlExpression)visitor.Visit(partition);
            changed |= newPartition != partition;
            partitions.Add(newPartition);
        }

        var orderings = new List<OrderingExpression>();
        foreach (var ordering in Orderings)
        {
            var newOrdering = (OrderingExpression)visitor.Visit(ordering);
            changed |= newOrdering != ordering;
            orderings.Add(newOrdering);
        }

        var expression = (SqlExpression)visitor.Visit(Expression);
        changed |= expression != Expression;

        return changed
            ? new WindowFunctionExpression(Function, expression, partitions, orderings, RowOrRange, TypeMapping)
            : this;
    }

    /// <inheritdoc />
    protected override void Print(ExpressionPrinter expressionPrinter)
    {
        ArgumentNullException.ThrowIfNull(expressionPrinter);
        expressionPrinter.Append($"{Function}(");
        expressionPrinter.Visit(Expression);
        expressionPrinter.Append(") OVER(");

        if (Partitions.Any())
        {
            expressionPrinter.Append("PARTITION BY ");
            expressionPrinter.VisitCollection(Partitions);
            expressionPrinter.Append(" ");
        }

        if (Orderings.Any())
        {
            expressionPrinter.Append("ORDER BY ");
            expressionPrinter.VisitCollection(Orderings);

            expressionPrinter.Visit(RowOrRange);
        }

        expressionPrinter.Append(")");
    }

    private bool Equals(WindowFunctionExpression windowFunctionsExpression)
        => base.Equals(windowFunctionsExpression)
            && Function.Equals(windowFunctionsExpression.Function, StringComparison.Ordinal)
            && (Partitions == null ? windowFunctionsExpression.Partitions == null : Partitions.SequenceEqual(windowFunctionsExpression.Partitions))
            && Orderings.SequenceEqual(windowFunctionsExpression.Orderings)
            && (RowOrRange == null ? windowFunctionsExpression.RowOrRange == null : RowOrRange.Equals(windowFunctionsExpression.RowOrRange));
}