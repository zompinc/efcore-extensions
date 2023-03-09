using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// Extensions for <see cref="RelationalQueryableMethodTranslatingExpressionVisitor"/>.
/// </summary>
public static class RelationalQueryableMethodTranslatingExpressionVisitorExtensions
{
    /// <summary>
    /// Translates custom methods like <see cref="SubQueryVisitorExtension.AsSubQuery{TEntity}"/>.
    /// </summary>
    /// <param name="visitor">The visitor.</param>
    /// <param name="methodCallExpression">Method call to translate.</param>
    /// <returns>Translated method call if a custom method is found; otherwise <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="visitor"/> or <paramref name="methodCallExpression"/> is <c>null</c>.
    /// </exception>
    public static Expression? TranslateRelationalMethods(
       this RelationalQueryableMethodTranslatingExpressionVisitor visitor,
       MethodCallExpression methodCallExpression)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        ArgumentNullException.ThrowIfNull(methodCallExpression);

        if (methodCallExpression.Method.DeclaringType == typeof(SubQueryVisitorExtension))
        {
            if (methodCallExpression.Method.Name == nameof(SubQueryVisitorExtension.AsSubQuery))
            {
                var expression = visitor.Visit(methodCallExpression.Arguments[0]);

                if (expression is ShapedQueryExpression shapedQueryExpression)
                {
                    ((SelectExpression)shapedQueryExpression.QueryExpression).PushdownIntoSubquery();
                    return shapedQueryExpression;
                }
            }
        }

        return null;
    }

    private static ShapedQueryExpression GetShapedQueryExpression(
       ExpressionVisitor visitor,
       MethodCallExpression methodCallExpression)
    {
        var source = visitor.Visit(methodCallExpression.Arguments[0]);

        if (source is not ShapedQueryExpression shapedQueryExpression)
        {
            throw new InvalidOperationException(CoreStrings.TranslationFailed(methodCallExpression.Print()));
        }

        return shapedQueryExpression;
    }
}
