namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Builds the window function calls for LINQ's aggregates, returning what LINQ returns.
/// </summary>
internal static class WindowAggregates
{
    private static readonly MethodInfo CountMethod = typeof(DbFunctionsExtensions).GetMethods()
        .Single(m => m.Name == nameof(DbFunctionsExtensions.Count) && !m.IsGenericMethod);

    /// <summary>
    /// COUNT(*) over <paramref name="over"/>.
    /// </summary>
    internal static Expression Count(Expression over) => Expression.Call(CountMethod, OverClauseBuilder.Functions, over);

    /// <summary>
    /// COUNT(*) over <paramref name="over"/> as a long. SQL Server's COUNT is an int whatever the query asks for, so it is cast.
    /// </summary>
    internal static Expression LongCount(Expression over) => Expression.Convert(Count(over), typeof(long));

    /// <summary>
    /// The aggregate <paramref name="name"/>, one of Sum, Min, Max and Average, of <paramref name="value"/> over
    /// <paramref name="over"/>, converted to <paramref name="resultType"/>.
    /// </summary>
    /// <remarks>
    /// LINQ averages integers as double, where SQL Server's AVG of an integer column is an integer, and sums a group of nulls
    /// to 0, where SUM returns NULL. The databases also disagree on the type of a sum of integers, int on SQL Server and bigint
    /// on PostgreSQL, so integers are summed as bigint, or decimal for a bigint column, and cast to the type LINQ returns.
    /// </remarks>
    /// <returns>The window function, or <see langword="null"/> when there is none for the type of the value.</returns>
    internal static Expression? Aggregate(string name, Expression value, Type resultType, Expression over)
    {
        var type = Nullable.GetUnderlyingType(value.Type) ?? value.Type;
        var widened = (name, Type.GetTypeCode(type)) switch
        {
            (nameof(Enumerable.Average), not (TypeCode.Decimal or TypeCode.Double)) => typeof(double),
            (nameof(Enumerable.Sum), TypeCode.Byte or TypeCode.SByte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32) => typeof(long),
            (nameof(Enumerable.Sum), TypeCode.Int64) => typeof(decimal),
            _ => null,
        };

        if (widened is not null)
        {
            value = Expression.Convert(value, Nullable.GetUnderlyingType(value.Type) is null ? widened : typeof(Nullable<>).MakeGenericType(widened));
        }

        if (WindowMethod(name == nameof(Enumerable.Average) ? nameof(DbFunctionsExtensions.Avg) : name, value.Type) is not { } method)
        {
            return null;
        }

        Expression call = Expression.Call(method, OverClauseBuilder.Functions, value, over);
        if (name == nameof(Enumerable.Sum) && Nullable.GetUnderlyingType(call.Type) is { } underlying)
        {
            call = Expression.Coalesce(call, Expression.Constant(Activator.CreateInstance(underlying), underlying));
        }

        return call.Type == resultType ? call : Expression.Convert(call, resultType);
    }

    /// <summary>
    /// Finds the window function <paramref name="name"/> taking a value of <paramref name="valueType"/>.
    /// </summary>
    private static MethodInfo? WindowMethod(string name, Type valueType)
    {
        var underlying = Nullable.GetUnderlyingType(valueType);
        foreach (var candidate in typeof(DbFunctionsExtensions).GetMethods())
        {
            if (candidate.Name != name || !candidate.IsGenericMethodDefinition || candidate.GetGenericArguments().Length != 1
                || candidate.GetParameters() is not [_, { ParameterType: var parameter }, _])
            {
                continue;
            }

            var takesNullable = parameter.IsGenericType && parameter.GetGenericTypeDefinition() == typeof(Nullable<>);
            if (takesNullable != (underlying is not null))
            {
                continue;
            }

            try
            {
                return candidate.MakeGenericMethod(underlying ?? valueType);
            }
            catch (ArgumentException)
            {
                // The type does not meet the candidate's constraint, such as a class for a struct overload.
            }
        }

        return null;
    }
}
