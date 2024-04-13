namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Creates Subquery for nested window functions and where clauses.
/// </summary>
public class WindowFunctionInsideWhereDetector : ExpressionVisitor
{
    private const string Original = "Original";
    private static readonly ConstructorInfo ConObj = typeof(object).GetConstructor([])
        ?? throw new InvalidOperationException("Can't be null");

    private static readonly Dictionary<string, List<MethodInfo>> QueryableMethodGroups = typeof(Enumerable)
        .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
        .GroupBy(mi => mi.Name)
        .ToDictionary(e => e.Key, l => l.ToList());

    /// <summary>
    /// Maps a where method call to window functions used inside it.
    /// </summary>
    private readonly Stack<Clause> clauseStack = new();

    private enum SubqueryType
    {
        Where,
        Select,
        OrderBy,
        OrderByDescending,
        ThenBy,
        ThenByDescending,
    }

    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        var suspendWhereStack = clauseStack.TryPeek(out var top) && top.FirstArgument == node ? top : null;

        if (suspendWhereStack is not null)
        {
            _ = clauseStack.Pop();
        }

        SubqueryType? subqueryType = node.Method.DeclaringType != typeof(Queryable) ? null
            : node.Method.Name switch
            {
                nameof(Queryable.Where) => SubqueryType.Where,
                nameof(Queryable.Select) => SubqueryType.Select,
                nameof(Queryable.OrderBy) => SubqueryType.OrderBy,
                nameof(Queryable.OrderByDescending) => SubqueryType.OrderByDescending,
                nameof(Queryable.ThenBy) => SubqueryType.ThenBy,
                nameof(Queryable.ThenByDescending) => SubqueryType.ThenByDescending,
                _ => null,
            };

        var wfNode = WindowFunctionsEvaluatableExpressionFilter.WindowFunctionMethods.Contains(node.Method, CompareNameAndDeclaringType.Default)
            ? node : null;

        _ = clauseStack.TryPeek(out var clause);

        if (wfNode is not null)
        {
            _ = clause ?? throw new InvalidOperationException("Window Function outside of select or where?");

            clause.Add(wfNode, clause.WindowFunctionStack.Count);

            clause.WindowFunctionStack.Push(wfNode);
        }

        if (subqueryType is not null)
        {
            clauseStack.Push(new(node));
        }

        var @base = (MethodCallExpression)base.VisitMethodCall(node);

        if (wfNode is not null)
        {
            _ = clause ?? throw new InvalidOperationException("Window Function outside of select or where?");
            _ = clause.WindowFunctionStack.Pop();
        }

        var method = @base.Method;
        if (subqueryType is not null)
        {
            var whereArg = @base.Arguments[1];
            var reduced = whereArg;

            while (reduced is UnaryExpression ue)
            {
                reduced = ue.Operand;
            }

            if (reduced is not LambdaExpression lambda)
            {
                throw new InvalidOperationException("Expression must be lambda");
            }

            var pop = clauseStack.Pop();
            if (pop.WindowFunctions.Count > 0)
            {
                @base = subqueryType == SubqueryType.Select
                    ? HandleSelectLambda(@base, lambda, pop)
                    : HandleLambda(@base, lambda, pop);
            }
        }

        if (suspendWhereStack is not null)
        {
            clauseStack.Push(suspendWhereStack);
        }

        return @base;
    }

    private static MethodCallExpression HandleSelectLambda(MethodCallExpression node, LambdaExpression originalLambda, Clause queryableMethod)
    {
        if (queryableMethod.WindowFunctions.Count < 2)
        {
            return node;
        }

        var expression = node.Arguments[0];
        var rewrittenLambda = BuildSubqueries(queryableMethod.WindowFunctions, originalLambda, ref expression);

        // Add .Select<MyAnon>(s => RowNumber(Over().OrderBy(s.P0)))
        var newSelectMethod = QueryableMethods.Select.MakeGenericMethod(expression.Type.GenericTypeArguments[0], node.Method.ReturnType.GenericTypeArguments[0]);
        var newSelect = Expression.Call(null, newSelectMethod, expression, rewrittenLambda);

        return newSelect;
    }

    private static MethodCallExpression HandleLambda(MethodCallExpression node, LambdaExpression originalLambda, Clause queryableMethod)
    {
        var methodInfo = queryableMethod.Method.Method;
        var isWhere = methodInfo.Name == nameof(Queryable.Where);

        if (queryableMethod.WindowFunctions.Count < 2 && !isWhere)
        {
            return node;
        }

        var expression = node.Arguments[0];
        var rewrittenLambda = BuildSubqueries(queryableMethod.WindowFunctions, originalLambda, ref expression, isWhere);

        var anonType = expression.Type.GenericTypeArguments[0];

        var newMethod = methodInfo.GetGenericMethodDefinition().MakeGenericMethod([anonType, .. methodInfo.GetGenericArguments()[1..]]);

        // Add .Where<MyAnon>(w => (w.P0 == 1))
        var newCall = Expression.Call(null, newMethod, expression, rewrittenLambda);

        // go back to original
        var b = Expression.Parameter(anonType, "b");
        var toOriginal = Expression.Property(b, Original);
        var trailingSelect = Expression.Lambda(toOriginal, b);
        var originalTypeArgument = node.Arguments[0].Type.GenericTypeArguments[0];
        var toOriginalMethod = QueryableMethods.Select.MakeGenericMethod(anonType, originalTypeArgument);

        // Add .Select<MyAnon>(b => b.Original)
        node = Expression.Call(null, toOriginalMethod, newCall, trailingSelect);
        return node;
    }

    private static LambdaExpression BuildSubqueries(
         IList<IList<MethodCallExpression>> windowFunctions,
         LambdaExpression lambda,
         ref Expression expression,
         bool isWhere = false)
    {
        var parameter = lambda.Parameters[0];
        var newParameter = parameter;
        var lastLevel = isWhere ? 0 : 1;
        var subqueryList = windowFunctions[^1].ToList();
        IDictionary<MethodCallExpression, Name_Type_And_Replacement> replacementMap;
        var level = windowFunctions.Count - 1;
        WindowFunctionRewriter wfr;

        while (true)
        {
            var typeArg = expression.Type.GenericTypeArguments[0];

            var replacements = new List<Name_Type_And_Replacement>();
            for (var i = 0; i < subqueryList.Count; ++i)
            {
                var item = subqueryList[i];
                replacements.Add(new($"P{i}", item.Type, item));
            }

            replacements.Insert(0, new(Original, parameter.Type, null!));

            var anonType = CreateNewType(replacements);
            var originalProperty = anonType.GetProperty(Original)
                ?? throw new InvalidOperationException("Can't be null, it was just created.");

            Expression originalValue = parameter == newParameter ? parameter : Expression.Property(newParameter, Original);
            var mbs = new List<MemberBinding>() { Expression.Bind(originalProperty, originalValue) };

            foreach (var item in replacements[1..])
            {
                var mi = anonType.GetMember(item.Name)[0]
                    ?? throw new InvalidOperationException($"{anonType} doesn't have property");
                mbs.Add(Expression.Bind(mi, item.Replacement));
            }

            var @new = Expression.New(anonType);
            var memberInit = Expression.MemberInit(@new, mbs);

            var innerSelectLambda = Expression.Lambda(memberInit, newParameter);

            // o => .Select<MyAnon>(o => new MyAnon() {Original = o, P0 = __Functions_0.RowNumber(__Over_1.OrderBy(o.Id))})
            var node = Expression.Call(
                null,
                QueryableMethods.Select.MakeGenericMethod(typeArg, anonType),
                expression,
                innerSelectLambda);

            var asSubQueryMethod = WindowFunctionsEvaluatableExpressionFilter.AsSubQueryMethod.MakeGenericMethod(anonType);

            // Add .AsSubQuery<MyAnon>()
            expression = Expression.Call(null, asSubQueryMethod, node);
            replacementMap = windowFunctions[level].Zip(replacements[1..])
                .ToDictionary(z => z.First, z => z.Second);
            newParameter = Expression.Parameter(expression.Type.GenericTypeArguments[0], $"{(level > 0 ? "l" + (level - 1) : "w")}");

            wfr = new(replacementMap, parameter, newParameter);

            if (--level < 0)
            {
                break;
            }

            var replacing = windowFunctions[level];
            subqueryList = replacing.Select(z => (MethodCallExpression)wfr.Visit(z)).ToList();
        }

        var newBody = wfr.Visit(lambda.Body);
        var rewrittenLambda = Expression.Lambda(newBody, newParameter);

        return rewrittenLambda;
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

    private sealed class WindowFunctionRewriter(IDictionary<MethodCallExpression, Name_Type_And_Replacement> map, ParameterExpression source, ParameterExpression target) : ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!map.TryGetValue(node, out var rec))
            {
                return base.VisitMethodCall(node);
            }

            var propExpression = Expression.Property(target, rec.Name);
            return propExpression;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node != source)
            {
                return base.VisitParameter(node);
            }

            var newProperty = Expression.Property(target, Original);
            return newProperty;
        }
    }

    /// <summary>
    /// Represents a method which could hold a Window function, such as .Select, .Where, .OdrerBy.
    /// </summary>
    /// <param name="Method">Method call expression.</param>
    private sealed record Clause(MethodCallExpression Method)
    {
        public Stack<MethodCallExpression> WindowFunctionStack { get; } = new();

        public IList<IList<MethodCallExpression>> WindowFunctions { get; } = [];

        public Expression FirstArgument => Method.Arguments[0];

        public void Add(MethodCallExpression call, int level)
        {
            while (WindowFunctions.Count == level)
            {
                WindowFunctions.Add([]);
            }

            WindowFunctions[level].Add(call);
        }
    }
}
