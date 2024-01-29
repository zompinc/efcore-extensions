namespace Zomp.EFCore.WindowFunctions.Query.Internal;

internal sealed class WindowFunctionDetector : ExpressionVisitor
{
    private static readonly ConstructorInfo ConObj = typeof(object).GetConstructor([])
        ?? throw new InvalidOperationException("Can't be null");

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
            if (method.Name == nameof(Queryable.Where))
            {
                var wfd = new WindowFunctionDetectorInternal();
                _ = wfd.Visit(node.Arguments[1]);

                var whereArg = node.Arguments[1];
                var reduced = whereArg;

                while (reduced is UnaryExpression ue)
                {
                    reduced = ue.Operand;
                }

                if (reduced is LambdaExpression whereLambda && wfd.WindowFunctionsCollection.Count > 0)
                {
                    var whereParam = whereLambda.Parameters[0];

                    var typeArg = node.Arguments[0].Type.GenericTypeArguments[0];
                    var p = Expression.Parameter(typeArg, "o");

                    var wfToSelect = wfd.WindowFunctionsCollection.Select(e => e.ReplaceParameter(whereParam, p)).ToList();

                    var list = new Dictionary<Expression, Name_Type_And_Replacement>();

                    for (var i = 0; i < wfd.WindowFunctionsCollection.Count; ++i)
                    {
                        var item = wfd.WindowFunctionsCollection[i];
                        list[item] = new($"P{i}", item.Type, wfToSelect[i]);
                    }

                    var replacements = list.Values.ToList();
                    replacements.Insert(0, new("Original", typeArg, null!));

                    var anonType = CreateNewType(replacements);

                    var originalProperty = anonType.GetProperty("Original")
                        ?? throw new InvalidOperationException("Can't be null, it was just created.");

                    var mbs = new List<MemberBinding>() { Expression.Bind(originalProperty, p) };

                    foreach (var item in list)
                    {
                        ////var item = zz.WindowFunctionsCollection[i];
                        ////mbs.Add(Expression.Bind())

                        var mi = anonType.GetMember(item.Value.Name)[0]
                            ?? throw new InvalidOperationException($"{anonType} doesn't have property");
                        mbs.Add(Expression.Bind(mi, item.Value.Replacement));
                    }

                    var @new = Expression.New(anonType);
                    var memberInit = Expression.MemberInit(@new, mbs);
                    var lambda = Expression.Lambda(memberInit, p);

                    node = Expression.Call(
                        null,
                        QueryableMethods.Select.MakeGenericMethod(typeArg, anonType),
                        node.Arguments[0],
                        lambda);

                    var asSubQueryMethod = WindowFunctionsEvaluatableExpressionFilter.AsSubQueryMethod.MakeGenericMethod(anonType);
                    node = Expression.Call(null, asSubQueryMethod, node);

                    p = Expression.Parameter(anonType, "w");
                    var wfr = new WindowFunctionRewriter(list, whereParam, p);
                    var body = wfr.Visit(whereLambda.Body);
                    var newWhere = Expression.Lambda(body, p);

                    var newWhereMethod = QueryableMethods.Where.MakeGenericMethod(anonType);
                    node = Expression.Call(null, newWhereMethod, node, newWhere);

                    // go back to original
                    p = Expression.Parameter(anonType, "b");
                    var toOriginal = Expression.Property(p, "Original");
                    var trailingSelect = Expression.Lambda(toOriginal, p);
                    var toOriginalMethod = QueryableMethods.Select.MakeGenericMethod(anonType, typeArg);

                    node = Expression.Call(null, toOriginalMethod, node, trailingSelect);
                    return node;
                }

                ////return call;
                ////return Expression.Call(node.Arguments[0], null!);
            }
        }

        if (WindowFunctionsEvaluatableExpressionFilter.WindowFunctionMethods.Contains(node.Method, CompareNameAndDeclaringType.Default))
        {
            WindowFunctionsCollection.Add(node);
        }

        var retVal = base.VisitMethodCall(node);

        return retVal;
    }

    ///// From : https://www.codeproject.com/Articles/121568/Dynamic-Type-Using-Reflection-Emit
    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "Not relevant")]
    private static Type CreateNewType(IEnumerable<Name_Type_And_Replacement> info)
    {
        // Let's start by creating a new assembly
        var dynamicAssemblyName = new AssemblyName("MyAsm");
        var dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(dynamicAssemblyName, AssemblyBuilderAccess.Run);
        var dynamicModule = dynamicAssembly.DefineDynamicModule("MyAsm");

        // Now let's build a new type
        var dynamicAnonymousType = dynamicModule.DefineType("MyAnon", TypeAttributes.Public);

        var cb = dynamicAnonymousType.DefineConstructor(
            MethodAttributes.Public |
            MethodAttributes.HideBySig |
            MethodAttributes.SpecialName |
            MethodAttributes.RTSpecialName,
            CallingConventions.Standard,
            []);

        var cil = cb.GetILGenerator();
        cil.Emit(OpCodes.Ldarg_0);
        cil.Emit(OpCodes.Call, ConObj);
        cil.Emit(OpCodes.Nop);

        foreach (var (name, type, _) in info)
        {
            var field = dynamicAnonymousType.DefineField(name.ToLowerInvariant(), type, FieldAttributes.Private);
            var property = dynamicAnonymousType.DefineProperty(name, PropertyAttributes.None, type, null);

            // getter
            var mFirstGet = dynamicAnonymousType.DefineMethod(
                $"get_{name}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                type,
                Type.EmptyTypes);

            // setter
            var mFirstSet = dynamicAnonymousType.DefineMethod(
                $"set_{name}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                null,
                [type]);

            var getterIl = mFirstGet.GetILGenerator();

            getterIl.Emit(OpCodes.Ldarg_0);
            getterIl.Emit(OpCodes.Ldfld, field);
            getterIl.Emit(OpCodes.Ret);

            var setterIl = mFirstSet.GetILGenerator();

            setterIl.Emit(OpCodes.Ldarg_0);
            setterIl.Emit(OpCodes.Ldarg_1);
            setterIl.Emit(OpCodes.Stfld, field);
            setterIl.Emit(OpCodes.Ret);

            property.SetGetMethod(mFirstGet);
            property.SetSetMethod(mFirstSet);
        }

        cil.Emit(OpCodes.Ret);

        // Return the type to the caller
        return dynamicAnonymousType.CreateType()!;
    }

    private static MethodInfo GetMethod(string name, int genericParameterCount, Func<Type[], Type[]> parameterGenerator)
    => QueryableMethodGroups[name].Single(
        mi => ((genericParameterCount == 0 && !mi.IsGenericMethod)
                || (mi.IsGenericMethod && mi.GetGenericArguments().Length == genericParameterCount))
            && mi.GetParameters().Select(e => e.ParameterType).SequenceEqual(
                parameterGenerator(mi.IsGenericMethod ? mi.GetGenericArguments() : [])));

    private sealed record Name_Type_And_Replacement(string Name, Type Type, Expression Replacement);

    private sealed class WindowFunctionRewriter(Dictionary<Expression, Name_Type_And_Replacement> map, ParameterExpression source, ParameterExpression target) : ExpressionVisitor
    {
        private readonly Dictionary<Expression, Name_Type_And_Replacement> map = map;
        private readonly ParameterExpression source = source;
        private readonly Expression target = target;

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (map.TryGetValue(node, out var rec))
            {
                var propExpression = Expression.Property(target, rec.Name);
                return propExpression;
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node != source)
            {
                return base.VisitParameter(node);
            }

            var newProperty = Expression.Property(target, "Original");
            return newProperty;
        }
    }

    private sealed class WindowFunctionDetectorInternal : ExpressionVisitor
    {
        public IList<MethodCallExpression> WindowFunctionsCollection { get; } = [];

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (WindowFunctionsEvaluatableExpressionFilter.WindowFunctionMethods.Contains(node.Method, CompareNameAndDeclaringType.Default))
            {
                WindowFunctionsCollection.Add(node);
            }

            return base.VisitMethodCall(node);
        }
    }

    private sealed class CompareNameAndDeclaringType : IEqualityComparer<MethodInfo>
    {
        public static CompareNameAndDeclaringType Default { get; } = new();

        public bool Equals(MethodInfo? x, MethodInfo? y)
        {
            if (x is null || y is null)
            {
                return x is null && y is null;
            }

            return x.Name.Equals(y.Name, StringComparison.Ordinal) && x.DeclaringType == y.DeclaringType;
        }

        public int GetHashCode(MethodInfo method)
        {
            return HashCode.Combine(method.Name.GetHashCode(StringComparison.Ordinal), method.DeclaringType?.GetHashCode());
        }
    }
}
