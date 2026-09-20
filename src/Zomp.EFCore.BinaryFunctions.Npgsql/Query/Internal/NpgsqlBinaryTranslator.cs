namespace Zomp.EFCore.BinaryFunctions.Npgsql.Query.Internal;

/// <summary>
/// A SQL translator for binary functions in Postgres.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NpgsqlBinaryTranslator"/> class.
/// </remarks>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
/// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
public class NpgsqlBinaryTranslator(ISqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource relationalTypeMappingSource) : BinaryTranslator(sqlExpressionFactory, relationalTypeMappingSource)
{
    private static readonly bool[] LPadArgumentsPropagateNullability = [true, false, false];
    private static readonly bool[] DecodeArgumentsPropagateNullabilityArray = [true, false];
    private static readonly bool[] ToHexArgumentsPropagateNullabilityArray = [true];

    /// <summary>
    /// Functions returning the binary representation PostgreSQL sends over the wire, which is big-endian.
    /// to_hex only takes integers, so every other type goes through one of these.
    /// </summary>
    private static readonly Dictionary<Type, string> SendFunctions = new()
    {
        [typeof(bool)] = "boolsend",
        [typeof(double)] = "float8send",
        [typeof(float)] = "float4send",
        [typeof(Guid)] = "uuid_send",
        [typeof(DateTime)] = "timestamp_send",
    };

    private readonly ISqlExpressionFactory sqlExpressionFactory = sqlExpressionFactory;
    private readonly RelationalTypeMapping? byteArrayTypeMapping = relationalTypeMappingSource.FindMapping(typeof(byte[]));

    /// <inheritdoc/>
    protected override SqlExpression BinaryCast(SqlExpression sqlExpression, Type toType)
    {
        ArgumentNullException.ThrowIfNull(sqlExpression);

        // A floating point number cannot be cast to bit(n). Its bytes can, as long as the target has as many.
        var fromType = Nullable.GetUnderlyingType(sqlExpression.Type) ?? sqlExpression.Type;
        if ((fromType == typeof(double) || fromType == typeof(float)) && Marshal.SizeOf(fromType) == Marshal.SizeOf(toType))
        {
            return ToValue(GetBytes(sqlExpression), toType);
        }

        var getBits = GetFixedBytes(sqlExpression, toType);
        if (toType == typeof(short))
        {
            // Without casting to int first Postgres outputs an error:
            // cannot cast type bit to smallint
            getBits = ToValue(getBits, typeof(int));
        }

        return ToValue(getBits, toType);
    }

    /// <inheritdoc/>
    protected override SqlExpression GetBytes(SqlExpression sqlExpression)
    {
        ArgumentNullException.ThrowIfNull(sqlExpression);

        if (SendFunctions.TryGetValue(Nullable.GetUnderlyingType(sqlExpression.Type) ?? sqlExpression.Type, out var sendFunction))
        {
            if (sqlExpression.TypeMapping?.StoreType == "timestamp with time zone")
            {
                sendFunction = "timestamptz_send";
            }

            var argument = sqlExpressionFactory.ApplyDefaultTypeMapping(sqlExpression);
            return sqlExpressionFactory.Function(sendFunction, [argument], true, ToHexArgumentsPropagateNullabilityArray, typeof(byte[]), byteArrayTypeMapping);
        }

        // Generate an expression like this: decode(LPAD(to_hex(r."SomeInt"), 8, '0'), 'hex')::bytea
        var toHex = sqlExpressionFactory.Function("to_hex", [sqlExpression], true, ToHexArgumentsPropagateNullabilityArray, typeof(string));
        var sizeOfType = Marshal.SizeOf(sqlExpression.Type);

        // Every byte is two characters in hex, thus multiply by 2
#if !EF_CORE_8
        var byteSize = new SqlConstantExpression(sizeOfType * 2, null);
        var zero = new SqlConstantExpression("0", null);
        var hex = new SqlConstantExpression("hex", null);
#else
        var byteSize = new SqlConstantExpression(Expression.Constant(sizeOfType * 2), null);
        var zero = new SqlConstantExpression(Expression.Constant("0"), null);
        var hex = new SqlConstantExpression(Expression.Constant("hex"), null);
#endif
        var lPad = sqlExpressionFactory.Function("LPAD", [toHex, byteSize, zero], true, LPadArgumentsPropagateNullability, typeof(string));
        var decode = sqlExpressionFactory.Function("decode", [lPad, hex], true, DecodeArgumentsPropagateNullabilityArray, typeof(string), byteArrayTypeMapping);
        return new SqlUnaryExpression(ExpressionType.Convert, decode, typeof(byte[]), byteArrayTypeMapping);
    }
}