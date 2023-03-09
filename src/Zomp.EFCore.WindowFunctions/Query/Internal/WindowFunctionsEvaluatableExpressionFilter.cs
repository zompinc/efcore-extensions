namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Evaluatable expression filter of ranking functions.
/// </summary>
public static class WindowFunctionsEvaluatableExpressionFilter
{
    private static readonly HashSet<MethodInfo> PreventEvaluationSet = new()
    {
        Info.OfMethod(ThisAssembly.AssemblyName, $"{ThisAssembly.RootNamespace}.{nameof(DbFunctionsExtensions)}", nameof(DbFunctionsExtensions.RowNumber)),
        Info.OfMethod(ThisAssembly.AssemblyName, $"{ThisAssembly.RootNamespace}.{nameof(DbFunctionsExtensions)}", nameof(DbFunctionsExtensions.Rank)),
        Info.OfMethod(ThisAssembly.AssemblyName, $"{ThisAssembly.RootNamespace}.{nameof(DbFunctionsExtensions)}", nameof(DbFunctionsExtensions.DenseRank)),
        Info.OfMethod(ThisAssembly.AssemblyName, $"{ThisAssembly.RootNamespace}.{nameof(DbFunctionsExtensions)}", nameof(DbFunctionsExtensions.PercentRank)),
    };

    /// <summary>
    /// Determines if expression should be compiled and evaluated.
    /// </summary>
    /// <param name="expression">Expression to evaluate.</param>
    /// <returns>false if expression should be filtered.</returns>
    public static bool IsEvaluatableExpression(Expression expression)
    {
        if (expression is MethodCallExpression methodCallExpression)
        {
            var declaringType = methodCallExpression.Method.DeclaringType;
            var method = methodCallExpression.Method;
            if (PreventEvaluationSet.Contains(method) && declaringType == typeof(DbFunctionsExtensions))
            {
                return false;
            }
        }

        return true;
    }
}
