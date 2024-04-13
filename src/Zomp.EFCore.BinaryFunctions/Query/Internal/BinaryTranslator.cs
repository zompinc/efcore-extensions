namespace Zomp.EFCore.BinaryFunctions.Query.Internal;

/// <summary>
/// A SQL translator for binary functions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BinaryTranslator"/> class.
/// </remarks>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
/// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
public class BinaryTranslator(ISqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource relationalTypeMappingSource) : IMethodCallTranslator
{
    private static readonly bool[] SubstringArgumentsPropagateNullability = [true, true, true];
    private readonly ISqlExpressionFactory sqlExpressionFactory = sqlExpressionFactory;
    private readonly IRelationalTypeMappingSource relationalTypeMappingSource = relationalTypeMappingSource;

    /// <inheritdoc/>
    public SqlExpression? Translate(SqlExpression? instance, MethodInfo method, IReadOnlyList<SqlExpression> arguments, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
    {
        ArgumentNullException.ThrowIfNull(method);

        return method.Name switch
        {
            nameof(DbFunctionsExtensions.GetBytes) => GetBytes(arguments[1]),
            nameof(DbFunctionsExtensions.Concat) => Concat(arguments),
            nameof(DbFunctionsExtensions.Substring) => Substring(arguments[1], arguments[2], arguments[3]),
            nameof(DbFunctionsExtensions.ToValue) when arguments.Count > 2 => ToValue(arguments[1], arguments[2], method.GetGenericArguments()[0]),
            nameof(DbFunctionsExtensions.ToValue) => ToValue(arguments[1], method.GetGenericArguments()[0]),
            nameof(DbFunctionsExtensions.BinaryCast) => BinaryCast(arguments[1], method.GetGenericArguments()[1]),
            _ => null,
        };
    }

    /// <summary>
    /// Casts from one type to another.
    /// </summary>
    /// <param name="sqlExpression">Expression to cast.</param>
    /// <param name="toType">Type to cast to.</param>
    /// <returns>The converted expression.</returns>
    protected virtual SqlExpression BinaryCast(SqlExpression sqlExpression, Type toType)
        => ToValue(GetBytes(sqlExpression), toType);

    /// <summary>
    /// Sql expression for <see cref="DbFunctionsExtensions.GetBytes{T}(DbFunctions, T?)"/> method.
    /// </summary>
    /// <param name="sqlExpression">Expression to get bytes for.</param>
    /// <returns>Sql expressions representing the binary data of the specified expression.</returns>
    protected virtual SqlExpression GetBytes(SqlExpression sqlExpression)
        => GetFixedBytes(sqlExpression, sqlExpression.Type);

    /// <summary>
    /// Represents sql expression of fixed size.
    /// </summary>
    /// <param name="sqlExpression">Expression to convert to bytes.</param>
    /// <param name="toType">Type of the expression.</param>
    /// <returns>Sql expression of fixed size.</returns>
    /// <remarks>
    /// Only use this method when toType is different from sqlExpression.Type. Otherwise use <see cref="GetBytes(SqlExpression)"/>.
    /// </remarks>
    protected SqlExpression GetFixedBytes(SqlExpression sqlExpression, Type toType)
    {
        var t = Type.MakeGenericSignatureType(typeof(FixedByteArray<>), [toType]);
        var mapping = relationalTypeMappingSource.FindMapping(t);
        return new SqlUnaryExpression(ExpressionType.Convert, sqlExpressionFactory.ApplyDefaultTypeMapping(sqlExpression), typeof(byte[]), mapping);
    }

    /// <summary>
    /// Sql expression for DbFunctionsExtensions.ToValue method.
    /// </summary>
    /// <param name="sqlExpression">Expression to convert to a CLR type.</param>
    /// <param name="type">Type to convert to.</param>
    /// <returns>SQL expression representing the conversion.</returns>
    protected virtual SqlExpression ToValue(SqlExpression sqlExpression, Type type)
        => new SqlUnaryExpression(ExpressionType.Convert, sqlExpression, type, null);

    private static SqlBinaryExpression Concat(IReadOnlyList<SqlExpression> arguments)
    {
        var left = arguments[1];
        var right = arguments[2];
        var expressionType = ExpressionType.Add;
        return new SqlBinaryExpression(expressionType, left, right, left.Type, null);
    }

    private SqlExpression ToValue(SqlExpression sqlExpression, SqlExpression offset, Type type)
        => ToValue(
            Substring(sqlExpression, offset, new SqlConstantExpression(Expression.Constant(Marshal.SizeOf(type)), null)),
            type);

    private SqlFunctionExpression Substring(SqlExpression bytearray, SqlExpression start, SqlExpression length) => sqlExpressionFactory.Function(
            "SUBSTRING",
            [
                bytearray,
                start,
                length,
            ],
            nullable: true,
            argumentsPropagateNullability: SubstringArgumentsPropagateNullability,
            bytearray.Type,
            null);
}