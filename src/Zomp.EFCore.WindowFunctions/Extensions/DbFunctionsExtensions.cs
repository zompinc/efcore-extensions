namespace Zomp.EFCore.WindowFunctions;

/// <summary>
/// Provides extension methods for binary types and window functions.
/// </summary>
public static partial class DbFunctionsExtensions
{
    private const string UseWindowFunctions = " One possible cause of this error is that '.UseWindowFunctions()' was not called on the 'DbContextOptionsBuilder' in 'OnConfiguring'.";

    /// <summary>
    /// The COUNT(*) window function returns the count of all rows.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Count for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static int Count(this DbFunctions _, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Count)) + UseWindowFunctions);

    /// <summary>
    /// The COUNT(*) window function returns the count of all rows.
    /// </summary>
    /// <typeparam name="TResult">Type of the result object.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="over">over clause.</param>
    /// <returns>Count for the selected window frame.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static TResult Count<TResult>(this DbFunctions _, OverClause over)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Count)) + UseWindowFunctions);

    /// <summary>
    /// Returns an instance of the Over clause.
    /// </summary>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <returns>The Over clause.</returns>
    public static OverClause Over(this DbFunctions _) => OverClause.Instance;

    /// <summary>
    /// Specifies order by expression for sorting in ascending order.
    /// </summary>
    /// <typeparam name="T">Type of the order by expression.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Order by expression.</param>
    /// <returns>The OrderByClause.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static OrderByClause OrderBy<T>(this OverClause _, T expression)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(OrderBy)) + UseWindowFunctions);

    /// <summary>
    /// Specifies order by expression for sorting in descending order.
    /// </summary>
    /// <typeparam name="T">Type of the order by expression.</typeparam>
    /// <param name="_">The <see cref="DbFunctions"/> instance.</param>
    /// <param name="expression">Order by expression.</param>
    /// <returns>The OrderByClause.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static OrderByClause OrderByDescending<T>(this OverClause _, T expression)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(OrderByDescending)) + UseWindowFunctions);

    /// <summary>
    /// Specifies subsequent order by expression for sorting in ascending order.
    /// </summary>
    /// <typeparam name="T">Type of the order by expression.</typeparam>
    /// <param name="_">The <see cref="OrderByClause"/> instance.</param>
    /// <param name="expression">Order by expression.</param>
    /// <returns>The OrderByClause.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static OrderByClause ThenBy<T>(this OrderByClause _, T expression)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(ThenBy)) + UseWindowFunctions);

    /// <summary>
    /// Specifies subsequent order by expression for sorting in descending order.
    /// </summary>
    /// <typeparam name="T">Type of the order by expression.</typeparam>
    /// <param name="_">The <see cref="OrderByClause"/> instance.</param>
    /// <param name="expression">Order by expression.</param>
    /// <returns>The OrderByClause.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static OrderByClause ThenByDescending<T>(this OrderByClause _, T expression)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(ThenByDescending)) + UseWindowFunctions);

    /// <summary>
    /// Specifies partiton by expression.
    /// </summary>
    /// <typeparam name="T">Type of the partition by expression.</typeparam>
    /// <param name="_">The <see cref="OverClause"/> instance.</param>
    /// <param name="expression">Partition by expression.</param>
    /// <returns>Partition by clause.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static PartitionByClause PartitionBy<T>(this OverClause _, T expression)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(PartitionBy)) + UseWindowFunctions);

    /// <summary>
    /// Specifies sunsequent partiton by expression.
    /// </summary>
    /// <typeparam name="T">Type of the partition by expression.</typeparam>
    /// <param name="_">The <see cref="PartitionByClause"/> instance.</param>
    /// <param name="expression">Subsequent partition by expression.</param>
    /// <returns>Partition by clause.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static PartitionByClause ThenBy<T>(this PartitionByClause _, T expression)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(ThenBy)) + UseWindowFunctions);

    /// <summary>
    /// Returns an instance of the Rows clause.
    /// </summary>
    /// <param name="_">The <see cref="OrderByClause"/> instance.</param>
    /// <returns>Rows clause.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static RowsClause Rows(this OrderByClause _)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Rows)) + UseWindowFunctions);

    /// <summary>
    /// Returns an instance of the Range clause.
    /// </summary>
    /// <param name="_">The <see cref="OrderByClause"/> instance.</param>
    /// <returns>Range clause.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static RangeClause Range(this OrderByClause _)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(Range)) + UseWindowFunctions);

    /// <summary>
    /// Represents the start of the analytics window with bound preceding.
    /// </summary>
    /// <param name="_">The <see cref="RowsClause"/> instance.</param>
    /// <param name="value">Value of preceding rows.</param>
    /// <returns>Start of the analytics window.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static OrderByClauseWithRowsOrRange FromPreceding(this RowsClause _, uint value)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(FromPreceding)) + UseWindowFunctions);

    /// <summary>
    /// Represents the start of the analytics window with bound following.
    /// </summary>
    /// <param name="_">The <see cref="RowsClause"/> instance.</param>
    /// <param name="value">Value of following rows.</param>
    /// <returns>Start of the analytics window.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    /// <remarks>
    /// Starts after the current row. Must be used with either <see cref="ToFollowing(IRangeCanBeClosed, uint)"/> or <see cref="ToUnbounded(IRangeCanBeClosed)"/>.
    /// </remarks>
    public static OrderByClauseWithRowsOrRangeNeedToClose FromFollowing(this RowsClause _, uint value)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(FromFollowing)) + UseWindowFunctions);

    /// <summary>
    /// Represents the end of the analytics window with bound preceding.
    /// </summary>
    /// <param name="_">The <see cref="OrderByClauseWithRowsOrRange"/> instance.</param>
    /// <param name="value">Value of preceding rows.</param>
    /// <returns>End of the analytics window.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static OrderByClauseWithRowsOrRange ToPreceding(this OrderByClauseWithRowsOrRange _, uint value)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(ToPreceding)) + UseWindowFunctions);

    /// <summary>
    /// Represents the end of the analytics window with bound following.
    /// </summary>
    /// <param name="_">The <see cref="OrderByClauseWithRowsOrRange"/> instance.</param>
    /// <param name="value">Value of following rows.</param>
    /// <returns>End of the analytics window.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static OrderByClauseWithRowsOrRange ToFollowing(this IRangeCanBeClosed _, uint value)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(ToFollowing)) + UseWindowFunctions);

    /// <summary>
    /// Represents the start of the analytics window with current row.
    /// </summary>
    /// <param name="_">The <see cref="RowsOrRangeClause"/> instance.</param>
    /// <returns>Start of the analytics window with current row.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static OrderByClauseWithRowsOrRange FromCurrentRow(this RowsOrRangeClause _)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(FromCurrentRow)) + UseWindowFunctions);

    /// <summary>
    /// Represents the end of the analytics window with current row.
    /// </summary>
    /// <param name="_">The <see cref="OrderByClauseWithRowsOrRange"/> instance.</param>
    /// <returns>End of the analytics window with current row.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static OrderByClauseWithRowsOrRange ToCurrentRow(this OrderByClauseWithRowsOrRange _)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(ToCurrentRow)) + UseWindowFunctions);

    /// <summary>
    /// Represents the start of the analytics window with unbounded preceding.
    /// </summary>
    /// <param name="_">The <see cref="RowsOrRangeClause"/> instance.</param>
    /// <returns>Start of the analytics window.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static OrderByClauseWithRowsOrRange FromUnbounded(this RowsOrRangeClause _)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(FromUnbounded)) + UseWindowFunctions);

    /// <summary>
    /// Represents the start of the analytics window with unbounded following.
    /// </summary>
    /// <param name="_">The <see cref="OrderByClauseWithRowsOrRange"/> instance.</param>
    /// <returns>End of the analytics window.</returns>
    /// <exception cref="InvalidOperationException">Occurs on client-side evaluation.</exception>
    public static OrderByClauseWithRowsOrRange ToUnbounded(this IRangeCanBeClosed _)
        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof(ToUnbounded)) + UseWindowFunctions);

    /// <summary>
    /// Executes provided query as a sub query.
    /// </summary>
    /// <param name="source">Query to execute as as sub query.</param>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// <returns>Query that will be executed as a sub query.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    internal static IQueryable<TEntity> AsSubQuery<TEntity>(this IQueryable<TEntity> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return source.Provider.CreateQuery<TEntity>(Expression.Call(null, WindowFunctionsEvaluatableExpressionFilter.AsSubQueryMethod.MakeGenericMethod(typeof(TEntity)), source.Expression));
    }
}