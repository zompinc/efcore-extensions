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
}