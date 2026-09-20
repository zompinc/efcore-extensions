namespace Zomp.EFCore.BinaryFunctions.Sqlite.Query.Internal;

/// <summary>
/// A SQL translator for binary functions in SQLite.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SqliteBinaryTranslator"/> class.
/// </remarks>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
/// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
public class SqliteBinaryTranslator(ISqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource relationalTypeMappingSource) : BinaryTranslator(sqlExpressionFactory, relationalTypeMappingSource)
{
    private static readonly bool[] OneArgumentPropagatesNullability = [true];
    private static readonly bool[] PrintfArgumentsPropagateNullability = [false, true];

    private readonly ISqlExpressionFactory sqlExpressionFactory = sqlExpressionFactory;
    private readonly RelationalTypeMapping? byteArrayTypeMapping = relationalTypeMappingSource.FindMapping(typeof(byte[]));

    /// <inheritdoc/>
    protected override SqlExpression BinaryCast(SqlExpression sqlExpression, Type toType)
    {
        var fromType = sqlExpression.Type;

        if (fromType == typeof(double) || fromType == typeof(float))
        {
            // FIXME: need implementation
            // Perhaps the opposite of http://multikoder.blogspot.com/2013/03/converting-varbinary-to-float-in-t-sql.html
            return base.BinaryCast(sqlExpression, toType);
        }

        var sizeInBytes = Marshal.SizeOf(toType);
        var maxValue = 1L << (sizeInBytes * 8);
        var maxValueSigned = 1L << ((sizeInBytes * 8) - 1);

#if !EF_CORE_8
        var maxValueSql = new SqlConstantExpression(maxValue, null);
        var maxValueSignedSql = new SqlConstantExpression(maxValueSigned, null);
#else
        var maxValueSql = new SqlConstantExpression(Expression.Constant(maxValue), null);
        var maxValueSignedSql = new SqlConstantExpression(Expression.Constant(maxValueSigned), null);
#endif

        // Equivalent of substring on binary data
        var modResult = new SqlBinaryExpression(ExpressionType.Modulo, sqlExpression, maxValueSql, fromType, null);

        // Convert from unsigned to Two's complement
        var addHalfRange = new SqlBinaryExpression(ExpressionType.Add, modResult, maxValueSignedSql, fromType, null);
        var modAgainResult = new SqlBinaryExpression(ExpressionType.Modulo, addHalfRange, maxValueSql, fromType, null);
        var subtractHalfRange = new SqlBinaryExpression(ExpressionType.Subtract, modAgainResult, maxValueSignedSql, fromType, null);

        return subtractHalfRange;
    }

    /// <inheritdoc/>
    protected override SqlExpression GetBytes(SqlExpression sqlExpression)
    {
        ArgumentNullException.ThrowIfNull(sqlExpression);

        var type = Nullable.GetUnderlyingType(sqlExpression.Type) ?? sqlExpression.Type;
        if (type != typeof(bool) && type != typeof(byte) && type != typeof(short) && type != typeof(int) && type != typeof(long))
        {
            return base.GetBytes(sqlExpression);
        }

        // CAST(1 AS BLOB) is the text '1'. SQLite has nothing that returns the bytes of a number, so they are
        // written as big-endian hex and read back: unhex(printf('%08X', "Id" & 4294967295)).
        // Masking is what makes a negative number come out at its own width and not as 64 bits.
        var size = type == typeof(bool) ? 1 : Marshal.SizeOf(type);
        var number = sqlExpressionFactory.ApplyDefaultTypeMapping(sqlExpression);
        var masked = size == sizeof(long)
            ? number
            : sqlExpressionFactory.And(sqlExpressionFactory.Convert(number, typeof(long)), sqlExpressionFactory.Constant((1L << (size * 8)) - 1));

        var hex = sqlExpressionFactory.Function("printf", [sqlExpressionFactory.Constant($"%0{size * 2}X"), masked], true, PrintfArgumentsPropagateNullability, typeof(string));
        var bytes = sqlExpressionFactory.Function("unhex", [hex], true, OneArgumentPropagatesNullability, typeof(byte[]), byteArrayTypeMapping);

        // printf prints NULL as 0.
        return sqlExpressionFactory.Case([new CaseWhenClause(sqlExpressionFactory.IsNull(number), sqlExpressionFactory.Constant(null, typeof(byte[]), byteArrayTypeMapping))], bytes);
    }

    /// <inheritdoc/>
    protected override SqlExpression Concat(SqlExpression left, SqlExpression right)
    {
        // || turns both blobs into text. Joining their hex keeps them binary: unhex(hex(left) || hex(right)).
        // hex(NULL) is an empty string, so NULL has to be handed on explicitly.
        var joined = sqlExpressionFactory.Add(Hex(left), Hex(right));
        var bytes = sqlExpressionFactory.Function("unhex", [joined], true, OneArgumentPropagatesNullability, typeof(byte[]), byteArrayTypeMapping);

        return sqlExpressionFactory.Case(
            [new CaseWhenClause(sqlExpressionFactory.OrElse(sqlExpressionFactory.IsNull(left), sqlExpressionFactory.IsNull(right)), sqlExpressionFactory.Constant(null, typeof(byte[]), byteArrayTypeMapping))],
            bytes);
    }

    private SqlExpression Hex(SqlExpression bytes)
        => sqlExpressionFactory.Function("hex", [bytes], true, OneArgumentPropagatesNullability, typeof(string));
}