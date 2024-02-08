using System.Collections.Frozen;

namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Evaluatable expression filter of ranking functions.
/// </summary>
public static class WindowFunctionsEvaluatableExpressionFilter
{
    private static readonly HashSet<string> WindowFunctionNames =
    [
        nameof(DbFunctionsExtensions.Avg),
        nameof(DbFunctionsExtensions.Count),
        nameof(DbFunctionsExtensions.DenseRank),
        nameof(DbFunctionsExtensions.Lag),
        nameof(DbFunctionsExtensions.Lead),
        nameof(DbFunctionsExtensions.Min),
        nameof(DbFunctionsExtensions.Max),
        nameof(DbFunctionsExtensions.PercentRank),
        nameof(DbFunctionsExtensions.Rank),
        nameof(DbFunctionsExtensions.RowNumber),
        nameof(DbFunctionsExtensions.Sum),
    ];

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Has to come after private fields")]
    internal static readonly FrozenSet<MethodInfo> WindowFunctionMethods =
        typeof(DbFunctionsExtensions).GetMethods().Where(x => WindowFunctionNames.Contains(x.Name)).ToFrozenSet();

    internal static readonly MethodInfo AsSubQueryMethod = Info.OfMethod(ThisAssembly.AssemblyName, $"{ThisAssembly.RootNamespace}.{nameof(DbFunctionsExtensions)}", nameof(DbFunctionsExtensions.AsSubQuery));

    private static readonly MethodInfo DenseRankMethod = Info.OfMethod(ThisAssembly.AssemblyName, $"{ThisAssembly.RootNamespace}.{nameof(DbFunctionsExtensions)}", nameof(DbFunctionsExtensions.DenseRank));
    private static readonly MethodInfo PercentRankMethod = Info.OfMethod(ThisAssembly.AssemblyName, $"{ThisAssembly.RootNamespace}.{nameof(DbFunctionsExtensions)}", nameof(DbFunctionsExtensions.PercentRank));
    private static readonly MethodInfo RankMethod = Info.OfMethod(ThisAssembly.AssemblyName, $"{ThisAssembly.RootNamespace}.{nameof(DbFunctionsExtensions)}", nameof(DbFunctionsExtensions.Rank));
    private static readonly MethodInfo RowNumberMethod = Info.OfMethod(ThisAssembly.AssemblyName, $"{ThisAssembly.RootNamespace}.{nameof(DbFunctionsExtensions)}", nameof(DbFunctionsExtensions.RowNumber));
    private static readonly MethodInfo CountMethod = Info.OfMethod(ThisAssembly.AssemblyName, $"{ThisAssembly.RootNamespace}.{nameof(DbFunctionsExtensions)}", nameof(DbFunctionsExtensions.Count));
    ////private static readonly MethodInfo CountTResultMethod = Info.OfMethod(ThisAssembly.AssemblyName, $"{ThisAssembly.RootNamespace}.{nameof(DbFunctionsExtensions)}", nameof(DbFunctionsExtensions.Count), "TResult");

    private static readonly HashSet<MethodInfo> PreventEvaluationSet =
    [
        RowNumberMethod,
        RankMethod,
        DenseRankMethod,
        PercentRankMethod,
        CountMethod,
        ////CountTResultMethod,
    ];

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
            if ((PreventEvaluationSet.Contains(method) || method.Name == nameof(DbFunctionsExtensions.Count))
                && declaringType == typeof(DbFunctionsExtensions))
            {
                return false;
            }
        }

        return true;
    }
}
