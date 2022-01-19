namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// A SQL translator for binary functions in Postgres.
/// </summary>
public class NpgsqlBinaryTranslator : BinaryTranslator
{
    private readonly ISqlExpressionFactory sqlExpressionFactory;
    private readonly RelationalTypeMapping? byteArrayTypeMapping;

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlBinaryTranslator"/> class.
    /// </summary>
    /// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
    /// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
    public NpgsqlBinaryTranslator(ISqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource relationalTypeMappingSource)
        : base(sqlExpressionFactory, relationalTypeMappingSource)
    {
        this.sqlExpressionFactory = sqlExpressionFactory;
        byteArrayTypeMapping = relationalTypeMappingSource.FindMapping(typeof(byte[]));
    }

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
        var toHex = sqlExpressionFactory.Function("to_hex", new[] { sqlExpression }, true, new[] { true }, typeof(string));
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
        var byteSize = new SqlConstantExpression(Expression.Constant(sizeOfType * 2), null);
        var zero = new SqlConstantExpression(Expression.Constant("0"), null);
        var hex = new SqlConstantExpression(Expression.Constant("hex"), null);
        var lPad = sqlExpressionFactory.Function("LPAD", new SqlExpression[] { toHex, byteSize, zero }, true, new[] { true, false, false }, typeof(string));
        var decode = sqlExpressionFactory.Function("decode", new SqlExpression[] { lPad, hex }, true, new[] { true, false }, typeof(string), byteArrayTypeMapping);
        return new SqlUnaryExpression(ExpressionType.Convert, decode, typeof(byte[]), byteArrayTypeMapping);
    }
}