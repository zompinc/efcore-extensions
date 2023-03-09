namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Temp.
/// </summary>
public static class SubQueryVisitorExtension
{
    private static readonly MethodInfo AsSubQueryMethodInfo = typeof(SubQueryVisitorExtension).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                                                     .Single(m => m.Name == nameof(AsSubQuery) && m.IsGenericMethod);

    /// <summary>
    /// Translates custom methods like <see cref="AsSubQuery{TEntity}"/>.
    /// </summary>
    /// <param name="visitor">The visitor.</param>
    /// <param name="methodCallExpression">Method call to translate.</param>
    /// <returns>Translated method call if a custom method is found; otherwise <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="visitor"/> or <paramref name="methodCallExpression"/> is <c>null</c>.
    /// </exception>
    public static ShapedQueryExpression? TranslateCustomMethods(this ExpressionVisitor visitor, MethodCallExpression methodCallExpression)
    {
        if (visitor == null)
        {
            throw new ArgumentNullException(nameof(visitor));
        }

        if (methodCallExpression == null)
        {
            throw new ArgumentNullException(nameof(methodCallExpression));
        }

        if (methodCallExpression.Method.DeclaringType == typeof(SubQueryVisitorExtension)
            && methodCallExpression.Method.Name == nameof(AsSubQuery))
        {
            var expression = visitor.Visit(methodCallExpression.Arguments[0]);

            if (expression is ShapedQueryExpression shapedQueryExpression)
            {
                ((SelectExpression)shapedQueryExpression.QueryExpression).PushdownIntoSubquery();
                return shapedQueryExpression;
            }
        }

        return null;
    }

    /// <summary>
    /// Executes provided query as a sub query.
    /// </summary>
    /// <param name="source">Query to execute as as sub query.</param>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// <returns>Query that will be executed as a sub query.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    public static IQueryable<TEntity> AsSubQuery<TEntity>(this IQueryable<TEntity> source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return source.Provider is EntityQueryProvider
                ? source.Provider.CreateQuery<TEntity>(Expression.Call(null, AsSubQueryMethodInfo.MakeGenericMethod(typeof(TEntity)), source.Expression))
                : source;
    }
}
