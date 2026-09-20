namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

/// <summary>
/// An expression that represents a Window Function in a SQL tree.
/// </summary>
/// <remarks>
/// Influenced by:
/// RowNumberExpression -
/// https://github.com/dotnet/efcore/blob/209865eb5d575b15da7aaff6e87078c00e727336/src/EFCore.Relational/Query/SqlExpressions/RowNumberExpression.cs
/// SqlServerAggregateFunctionExpression -
/// https://github.com/dotnet/efcore/blob/209865eb5d575b15da7aaff6e87078c00e727336/src/EFCore.SqlServer/Query/Internal/SqlServerAggregateFunctionExpression.cs.
/// </remarks>
public class WindowFunctionExpression : SqlExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionExpression"/> class whose type is the type of its first argument.
    /// </summary>
    /// <param name="function">Function (MIN, MAX).</param>
    /// <param name="arguments">A list of argument expressions of the Window function.</param>
    /// <param name="nullHandling">Respect or ignore nulls.</param>
    /// <param name="partitions">A list of expressions to partition by.</param>
    /// <param name="orderings">A list of ordering expressions to order by.</param>
    /// <param name="rowOrRange">Row or range clause.</param>
    /// <param name="typeMapping">The <see cref="RelationalTypeMapping" /> associated with the expression.</param>
    public WindowFunctionExpression(
        string function,
        IReadOnlyList<SqlExpression> arguments,
        NullHandling? nullHandling,
        IReadOnlyList<SqlExpression>? partitions,
        IReadOnlyList<OrderingExpression>? orderings,
        RowOrRangeExpression? rowOrRange,
        RelationalTypeMapping? typeMapping)
        : this(function, arguments, nullHandling, partitions, orderings, rowOrRange, typeMapping, InferType(function ?? throw new ArgumentNullException(nameof(function)), arguments))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionExpression"/> class.
    /// </summary>
    /// <param name="function">Function (MIN, MAX).</param>
    /// <param name="arguments">A list of argument expressions of the Window function.</param>
    /// <param name="nullHandling">Respect or ignore nulls.</param>
    /// <param name="partitions">A list of expressions to partition by.</param>
    /// <param name="orderings">A list of ordering expressions to order by.</param>
    /// <param name="rowOrRange">Row or range clause.</param>
    /// <param name="typeMapping">The <see cref="RelationalTypeMapping" /> associated with the expression.</param>
    /// <param name="type">The type of the result, for functions such as COUNT where it is not the type of the argument.</param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "The other constructor has to compute the type first")]
    public WindowFunctionExpression(
        string function,
        IReadOnlyList<SqlExpression> arguments,
        NullHandling? nullHandling,
        IReadOnlyList<SqlExpression>? partitions,
        IReadOnlyList<OrderingExpression>? orderings,
        RowOrRangeExpression? rowOrRange,
        RelationalTypeMapping? typeMapping,
        Type type)
        : base(type, typeMapping)
    {
        Function = function;
        Arguments = arguments;
        NullHandling = nullHandling;
        Partitions = partitions ?? [];
        Orderings = orderings ?? [];
        RowOrRange = rowOrRange;
    }

    /// <summary>
    /// Gets the arguments of the window function.
    /// </summary>
    public virtual IReadOnlyList<SqlExpression> Arguments { get; }

    /// <summary>
    /// Gets the respect nulls or ignore nulls parameter.
    /// </summary>
    public NullHandling? NullHandling { get; }

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
                : new(Function, arguments, NullHandling, partitions, orderings, RowOrRange, TypeMapping, Type);

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

#if !EF_CORE_8
    /// <inheritdoc/>
    public override Expression Quote() => throw new NotImplementedException();
#endif

    /// <inheritdoc />
    protected override Expression VisitChildren(ExpressionVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var changed = false;
        var partitions = new List<SqlExpression>();
        var arguments = new List<SqlExpression>();
        foreach (var argument in Arguments)
        {
            var newEArgument = (SqlExpression)visitor.Visit(argument);
            changed |= newEArgument != argument;
            arguments.Add(newEArgument);
        }

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

        return changed
            ? new WindowFunctionExpression(Function, arguments, NullHandling, partitions, orderings, RowOrRange, TypeMapping, Type)
            : this;
    }

    /// <inheritdoc />
    protected override void Print(ExpressionPrinter expressionPrinter)
    {
        ArgumentNullException.ThrowIfNull(expressionPrinter);
        _ = expressionPrinter.Append($"{Function}(");
        if (Arguments.Count == 0
            && Function.Equals(nameof(DbFunctionsExtensions.Count), StringComparison.OrdinalIgnoreCase))
        {
            _ = expressionPrinter.Append($"*");
        }
        else
        {
            expressionPrinter.VisitCollection(Arguments);
        }

        _ = expressionPrinter.Append(") ");

        if (NullHandling is { } nullHandling)
        {
            _ = expressionPrinter.Append(nullHandling == WindowFunctions.NullHandling.RespectNulls
                ? "RESPECT NULLS " : "IGNORE NULLS ");
        }

        _ = expressionPrinter.Append("OVER(");

        if (Partitions.Count > 0)
        {
            _ = expressionPrinter.Append("PARTITION BY ");
            expressionPrinter.VisitCollection(Partitions);
            _ = expressionPrinter.Append(" ");
        }

        if (Orderings.Count > 0)
        {
            _ = expressionPrinter.Append("ORDER BY ");
            expressionPrinter.VisitCollection(Orderings);

            _ = expressionPrinter.Visit(RowOrRange);
        }

        _ = expressionPrinter.Append(")");
    }

    private static Type InferType(string function, IReadOnlyList<SqlExpression> arguments)
        => arguments is [var e, ..] ? e.Type
            : function.Equals(nameof(DbFunctionsExtensions.Count), StringComparison.OrdinalIgnoreCase) ? typeof(int)
            : typeof(long);

    private bool Equals(WindowFunctionExpression windowFunctionsExpression)
        => base.Equals(windowFunctionsExpression)
            && ((Arguments is null && windowFunctionsExpression.Arguments is null) || (Arguments?.Equals(windowFunctionsExpression.Arguments) ?? false))
            && Function.Equals(windowFunctionsExpression.Function, StringComparison.Ordinal)
            && (Partitions == null ? windowFunctionsExpression.Partitions == null : Partitions.SequenceEqual(windowFunctionsExpression.Partitions))
            && Orderings.SequenceEqual(windowFunctionsExpression.Orderings)
            && (RowOrRange == null ? windowFunctionsExpression.RowOrRange == null : RowOrRange.Equals(windowFunctionsExpression.RowOrRange));
}