namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

/// <summary>
/// An expression that represents a Window Function in a SQL tree.
/// </summary>
/// <param name="function">Function (MIN, MAX).</param>
/// <param name="expressions">The expression of the Window function.</param>
/// <param name="partitions">A list expressions to partition by.</param>
/// <param name="orderings">A list of ordering expressions to order by.</param>
/// <param name="rowOrRange">Row or range clause.</param>
/// <param name="typeMapping">The <see cref="RelationalTypeMapping" /> associated with the expression.</param>
/// <remarks>
/// Influenced by:
/// RowNumberExpression -
/// https://github.com/dotnet/efcore/blob/209865eb5d575b15da7aaff6e87078c00e727336/src/EFCore.Relational/Query/SqlExpressions/RowNumberExpression.cs
/// SqlServerAggregateFunctionExpression -
/// https://github.com/dotnet/efcore/blob/209865eb5d575b15da7aaff6e87078c00e727336/src/EFCore.SqlServer/Query/Internal/SqlServerAggregateFunctionExpression.cs.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionExpression"/> class.
/// </remarks>
public class WindowFunctionExpression(
    string function,
    IReadOnlyList<SqlExpression> expressions,
    IReadOnlyList<SqlExpression>? partitions,
    IReadOnlyList<OrderingExpression>? orderings,
    RowOrRangeExpression? rowOrRange,
    RelationalTypeMapping? typeMapping) : SqlExpression(expressions is [var e, ..] ? e.Type : typeof(long), typeMapping)
{
    /// <summary>
    /// Gets the expression of the window function.
    /// </summary>
    public virtual IReadOnlyList<SqlExpression> Expressions { get; } = expressions;

    /// <summary>
    /// Gets the list of expressions used in partitioning.
    /// </summary>
    public virtual IReadOnlyList<SqlExpression> Partitions { get; } = partitions ?? Array.Empty<SqlExpression>();

    /// <summary>
    /// Gets list of ordering expressions used to order inside the given partition.
    /// </summary>
    public virtual IReadOnlyList<OrderingExpression> Orderings { get; } = orderings ?? Array.Empty<OrderingExpression>();

    /// <summary>
    /// Gets the function name.
    /// </summary>
    /// <remarks>
    /// Possible values: Max / Min...
    /// </remarks>
    public string Function { get; } = function;

    /// <summary>
    /// Gets the Row or Range clause.
    /// </summary>
    public RowOrRangeExpression? RowOrRange { get; } = rowOrRange;

    /// <summary>
    /// Updates.
    /// </summary>
    /// <param name="expressions">The expression of the Window function.</param>
    /// <param name="partitions">A list of partition expressions to partition by.</param>
    /// <param name="orderings">A list of ordering expressions to order by.</param>
    /// <returns>Updated expression.</returns>
    public WindowFunctionExpression Update(IReadOnlyList<SqlExpression> expressions, IReadOnlyList<SqlExpression> partitions, IReadOnlyList<OrderingExpression> orderings)
        => (ReferenceEquals(expressions, Expressions) || expressions.SequenceEqual(Expressions))
            && (ReferenceEquals(partitions, Partitions) || partitions.SequenceEqual(Partitions))
            && (ReferenceEquals(orderings, Orderings) || orderings.SequenceEqual(Orderings))
                ? this
                : new(Function, expressions, partitions, orderings, RowOrRange, TypeMapping);

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

        hash.Add(Expressions);
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

        var expressions = new List<SqlExpression>();
        foreach (var expression in Expressions)
        {
            var newExpression = (SqlExpression)visitor.Visit(expression);
            changed |= newExpression != expression;
            expressions.Add(newExpression);
        }

        return changed
            ? new WindowFunctionExpression(Function, expressions, partitions, orderings, RowOrRange, TypeMapping)
            : this;
    }

    /// <inheritdoc />
    protected override void Print(ExpressionPrinter expressionPrinter)
    {
        ArgumentNullException.ThrowIfNull(expressionPrinter);
        expressionPrinter.Append($"{Function}(");
        expressionPrinter.VisitCollection(Expressions);
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
            && ((Expressions is null && windowFunctionsExpression.Expressions is null) || (Expressions?.Equals(windowFunctionsExpression.Expressions) ?? false))
            && Function.Equals(windowFunctionsExpression.Function, StringComparison.Ordinal)
            && (Partitions == null ? windowFunctionsExpression.Partitions == null : Partitions.SequenceEqual(windowFunctionsExpression.Partitions))
            && Orderings.SequenceEqual(windowFunctionsExpression.Orderings)
            && (RowOrRange == null ? windowFunctionsExpression.RowOrRange == null : RowOrRange.Equals(windowFunctionsExpression.RowOrRange));
}