namespace Zomp.EFCore.WindowFunctions.Query.Internal;

internal sealed class JoinDetector : ExpressionVisitor
{
    private static readonly Dictionary<string, List<MethodInfo>> QueryableMethodGroups = typeof(Enumerable)
        .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
        .GroupBy(mi => mi.Name)
        .ToDictionary(e => e.Key, l => l.ToList());

    /*
    private static readonly MethodInfo Where = GetMethod(
            nameof(Enumerable.Where),
            1,
            types => new[] { typeof(IEnumerable<>).MakeGenericType(types[0]), typeof(Func<,>).MakeGenericType(types[0], typeof(bool)) });
    */

    public IList<MethodCallExpression> WindowFunctionsCollection { get; } = new List<MethodCallExpression>();

    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        var method = node.Method;
        if (method.DeclaringType == typeof(Queryable))
        {
            if (method.Name == nameof(Queryable.Join))
            {
                var wfd = new WindowFunctionDetectorInternal();
                var joinArg = node.Arguments[1];

                _ = wfd.Visit(joinArg);

                if (wfd.WindowFunctionsCollection.Count == 0)
                {
                    return base.VisitMethodCall(node);
                }

                var @base = (MethodCallExpression)base.VisitMethodCall(node);

                var type = QueryableMethods.Join.MakeGenericMethod(method.GetGenericArguments());

                var args = @base.Arguments;

                var argForSubQuery = args[1];

                var asSubQueryMethod = WindowFunctionsEvaluatableExpressionFilter.AsSubQueryMethod.MakeGenericMethod(argForSubQuery.Type.GetGenericArguments()[0]);
                var withSubquery = Expression.Call(null, asSubQueryMethod, argForSubQuery);

                var joinWithSubquery = Expression.Call(
                    null,
                    method,
                    args[0],
                    withSubquery,
                    args[2],
                    args[3],
                    args[4]);
                return joinWithSubquery;
            }
        }

        if (WindowFunctionsEvaluatableExpressionFilter.WindowFunctionMethods.Contains(node.Method, CompareNameAndDeclaringType.Default))
        {
            WindowFunctionsCollection.Add(node);
        }

        var retVal = base.VisitMethodCall(node);

        return retVal;
    }

    private static MethodInfo GetMethod(string name, int genericParameterCount, Func<Type[], Type[]> parameterGenerator)
    => QueryableMethodGroups[name].Single(
        mi => ((genericParameterCount == 0 && !mi.IsGenericMethod)
                || (mi.IsGenericMethod && mi.GetGenericArguments().Length == genericParameterCount))
            && mi.GetParameters().Select(e => e.ParameterType).SequenceEqual(
                parameterGenerator(mi.IsGenericMethod ? mi.GetGenericArguments() : [])));
}
