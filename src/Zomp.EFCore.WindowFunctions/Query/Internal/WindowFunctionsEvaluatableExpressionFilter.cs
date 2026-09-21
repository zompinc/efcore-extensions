namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Evaluatable expression filter of ranking functions.
/// </summary>
public static class WindowFunctionsEvaluatableExpressionFilter
{
    /// <summary>
    /// The window functions: extensions of <see cref="DbFunctions"/> that take an <see cref="OverClause"/>. Derived rather than
    /// listed so that a new function is pushed into a subquery inside Where, a join or an aggregate without being registered here.
    /// </summary>
    internal static readonly FrozenSet<MethodInfo> WindowFunctionMethods =
        typeof(DbFunctionsExtensions).GetMethods()
            .Where(m => m.GetParameters() is [{ ParameterType: var first }, ..] parameters
                && first == typeof(DbFunctions)
                && parameters.Any(p => p.ParameterType == typeof(OverClause)))
            .ToFrozenSet();

    internal static readonly MethodInfo AsSubQueryMethod = Info.OfMethod(ThisAssembly.AssemblyName, $"{ThisAssembly.RootNamespace}.{nameof(DbFunctionsExtensions)}", nameof(DbFunctionsExtensions.AsSubQuery));

    /// <summary>
    /// Determines if expression should be compiled and evaluated.
    /// </summary>
    /// <param name="expression">Expression to evaluate.</param>
    /// <returns>false if expression should be filtered.</returns>
    public static bool IsEvaluatableExpression(Expression expression)
    {
        // A window function only runs in the database, even when nothing in the call depends on the row.
        return expression is not MethodCallExpression { Method: var method }
            || !WindowFunctionMethods.Contains(method, CompareNameAndDeclaringType.Default);
    }
}
