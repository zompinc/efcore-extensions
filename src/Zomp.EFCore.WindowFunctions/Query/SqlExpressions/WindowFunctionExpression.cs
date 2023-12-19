﻿namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

/// <summary>
/// An expression that represents a Window Function in a SQL tree.
/// </summary>
/// <param name="function">Function (MIN, MAX).</param>
/// <param name="arguments">A list of argument expressions of the Window function.</param>
/// <param name="partitions">A list of expressions to partition by.</param>
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
    IReadOnlyList<SqlExpression> arguments,
    IReadOnlyList<SqlExpression>? partitions,
    IReadOnlyList<OrderingExpression>? orderings,
    RowOrRangeExpression? rowOrRange,
    RelationalTypeMapping? typeMapping) : SqlExpression(arguments is [var e, ..] ? e.Type : typeof(long), typeMapping)
{
    /// <summary>
    /// Gets the arguments of the window function.
    /// </summary>
    public virtual IReadOnlyList<SqlExpression> Arguments { get; } = arguments;

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
    /// <param name="arguments">The expression of the Window function.</param>
    /// <param name="partitions">A list of partition expressions to partition by.</param>
    /// <param name="orderings">A list of ordering expressions to order by.</param>
    /// <returns>Updated expression.</returns>
    public WindowFunctionExpression Update(IReadOnlyList<SqlExpression> arguments, IReadOnlyList<SqlExpression> partitions, IReadOnlyList<OrderingExpression> orderings)
        => (ReferenceEquals(arguments, Arguments) || arguments.SequenceEqual(Arguments))
            && (ReferenceEquals(partitions, Partitions) || partitions.SequenceEqual(Partitions))
            && (ReferenceEquals(orderings, Orderings) || orderings.SequenceEqual(Orderings))
                ? this
                : new(Function, arguments, partitions, orderings, RowOrRange, TypeMapping);

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

        hash.Add(Arguments);
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

        var arguments = new List<SqlExpression>();
        foreach (var argument in Arguments)
        {
            var newExpression = (SqlExpression)visitor.Visit(argument);
            changed |= newExpression != argument;
            arguments.Add(newExpression);
        }

        return changed
            ? new WindowFunctionExpression(Function, arguments, partitions, orderings, RowOrRange, TypeMapping)
            : this;
    }

    /// <inheritdoc />
    protected override void Print(ExpressionPrinter expressionPrinter)
    {
        ArgumentNullException.ThrowIfNull(expressionPrinter);
        expressionPrinter.Append($"{Function}(");
        expressionPrinter.VisitCollection(Arguments);
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
            && ((Arguments is null && windowFunctionsExpression.Arguments is null) || (Arguments?.Equals(windowFunctionsExpression.Arguments) ?? false))
            && Function.Equals(windowFunctionsExpression.Function, StringComparison.Ordinal)
            && (Partitions == null ? windowFunctionsExpression.Partitions == null : Partitions.SequenceEqual(windowFunctionsExpression.Partitions))
            && Orderings.SequenceEqual(windowFunctionsExpression.Orderings)
            && (RowOrRange == null ? windowFunctionsExpression.RowOrRange == null : RowOrRange.Equals(windowFunctionsExpression.RowOrRange));
}