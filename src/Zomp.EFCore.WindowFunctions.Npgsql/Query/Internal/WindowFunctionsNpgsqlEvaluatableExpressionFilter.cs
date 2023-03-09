using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;
using Zomp.EFCore.WindowFunctions.Clauses;

namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// Evaluatable expression filter for Npgsql.
/// </summary>
public class WindowFunctionsNpgsqlEvaluatableExpressionFilter : NpgsqlEvaluatableExpressionFilter
{
    private static readonly MethodInfo QueryRowNumberOver = typeof(DbFunctionsExtensions).GetRuntimeMethod(nameof(DbFunctionsExtensions.RowNumber), new Type[] { typeof(DbFunctions), typeof(OverClause) })
        ?? throw new InvalidOperationException("Type should be found");

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlEvaluatableExpressionFilter"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="relationalDependencies">Relational service dependencies.</param>
    public WindowFunctionsNpgsqlEvaluatableExpressionFilter(EvaluatableExpressionFilterDependencies dependencies, RelationalEvaluatableExpressionFilterDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
    {
    }

    /// <inheritdoc/>
    public override bool IsEvaluatableExpression(Expression expression, IModel model)
    {
        if (expression is MethodCallExpression methodCallExpression)
        {
            var declaringType = methodCallExpression.Method.DeclaringType;
            var method = methodCallExpression.Method;
            if (method == QueryRowNumberOver && declaringType == typeof(DbFunctionsExtensions))
            {
                return false;
            }
        }

        return base.IsEvaluatableExpression(expression, model);
    }
}
