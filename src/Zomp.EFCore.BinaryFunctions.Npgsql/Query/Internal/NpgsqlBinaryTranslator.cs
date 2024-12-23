﻿namespace Zomp.EFCore.BinaryFunctions.Npgsql.Query.Internal;

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

    private readonly ISqlExpressionFactory sqlExpressionFactory = sqlExpressionFactory;
    private readonly RelationalTypeMapping? byteArrayTypeMapping = relationalTypeMappingSource.FindMapping(typeof(byte[]));

    /// <inheritdoc/>
    protected override SqlExpression BinaryCast(SqlExpression sqlExpression, Type toType)
    {
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
        // Generate an expression like this: decode(LPAD(to_hex(r."SomeInt"), 8, '0'), 'hex')::bytea
        var toHex = sqlExpressionFactory.Function("to_hex", [sqlExpression], true, ToHexArgumentsPropagateNullabilityArray, typeof(string));
        var type = sqlExpression.Type;
        int sizeOfType;
        if (type == typeof(DateTime))
        {
            // fixme: refer to date/time types
            // https://www.postgresql.org/docs/current/datatype-datetime.html
            sizeOfType = 8;
        }
        else
        {
            sizeOfType = Marshal.SizeOf(sqlExpression.Type);
        }

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