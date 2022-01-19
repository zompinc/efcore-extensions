namespace Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

/// <summary>
/// An expression that represents rows or range clause of the over clause.
/// </summary>
public class RowOrRangeExpression : SqlExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RowOrRangeExpression"/> class.
    /// </summary>
    /// <param name="isRows">"ROWS" or "RANGE" keyword.</param>
    /// <param name="start">starting window frame.</param>
    /// <param name="end">ending window frame.</param>
    public RowOrRangeExpression(bool isRows, WindowFrame start, WindowFrame? end = null)

        : base(typeof(RowOrRangeExpression), null)
    {
        IsRows = isRows;
        Start = start;
        End = end;
    }

    /// <summary>
    /// Gets a value indicating whether "ROWS" or "RANGE" is specified.
    /// </summary>
    public bool IsRows { get; }

    /// <summary>
    /// Gets the starting window frame.
    /// </summary>
    public WindowFrame Start { get; }

    /// <summary>
    /// Gets the ending window frame.
    /// </summary>
    public WindowFrame? End { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj != null
            && (ReferenceEquals(this, obj)
                || (obj is RowOrRangeExpression rowOrRangeClause
                && Equals(rowOrRangeClause)));

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = default(HashCode);
        hash.Add(base.GetHashCode());
        hash.Add(IsRows);
        hash.Add(Start);
        hash.Add(End);
        return hash.ToHashCode();
    }

    /// <inheritdoc/>
    protected override void Print(ExpressionPrinter expressionPrinter)
    {
        ArgumentNullException.ThrowIfNull(expressionPrinter);
        expressionPrinter.Append(" ");

        expressionPrinter.Append(IsRows ? "ROWS " : "RANGE ");

        if (End is { } following)
        {
            expressionPrinter.Append("BETWEEN ");
            ProcessWindowFrame(expressionPrinter, Start, false);
            expressionPrinter.Append(" AND ");
            ProcessWindowFrame(expressionPrinter, End, true);
            return;
        }

        ProcessWindowFrame(expressionPrinter, Start, false);
    }

    private static void ProcessWindowFrame(ExpressionPrinter expressionPrinter, WindowFrame windowFrame, bool isStart)
    {
        expressionPrinter.Append(windowFrame.ToString()!);

        if (windowFrame.IsDirectional)
        {
            expressionPrinter.Append(" ");
            bool isFollowing = windowFrame is BoundedWindowFrame bwf ? bwf.IsFollowing : isStart;
            expressionPrinter.Append(isFollowing ? "FOLLOWING" : "PRECEDING");
        }
    }

    private bool Equals(RowOrRangeExpression rowOrRangeClause)
    => base.Equals(rowOrRangeClause)
        && IsRows.Equals(rowOrRangeClause.IsRows)
        && Start.Equals(rowOrRangeClause.Start)
        && (End == null ? rowOrRangeClause.End == null : End.Equals(rowOrRangeClause.End));
}