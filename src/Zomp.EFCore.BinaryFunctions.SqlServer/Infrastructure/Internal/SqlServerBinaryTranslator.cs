namespace Zomp.EFCore.BinaryFunctions.SqlServer.Infrastructure.Internal;

/// <summary>
/// A SQL translator for binary functions in SQL Server.
/// </summary>
public class SqlServerBinaryTranslator : BinaryTranslator
{
    private static readonly bool[] SignArgumentsPropagateNullability = new[] { true };
    private static readonly bool[] PowerArgumentsPropagateNullability = new[] { false, false };

    // Constants
    private static readonly double TwoToThePowerOfMinus52 = Math.Pow(2d, -52);
    private static readonly SqlExpression TwoToThePowerOfMinus52Sql = new SqlConstantExpression(Expression.Constant(TwoToThePowerOfMinus52), null);
    private static readonly SqlExpression OneFloat = new SqlConstantExpression(Expression.Constant(1d), null);
    private static readonly SqlExpression TwoFloat = new SqlConstantExpression(Expression.Constant(2d), null);

    // Must be varbinary.
    private static readonly SqlExpression X000FFFFFFFFFFFFF = new SqlFragmentExpression("0x000FFFFFFFFFFFFF");
    private static readonly SqlExpression X7ff0000000000000 = new SqlFragmentExpression("0x7ff0000000000000");
    private static readonly SqlExpression X0010000000000000 = new SqlFragmentExpression("0x0010000000000000");

    // Other providers can use this:
    /*
        var x000FFFFFFFFFFFFF = GetBytes(new SqlConstantExpression(Expression.Constant(0x000FFFFFFFFFFFFF), null));
        var x7ff0000000000000 = GetBytes(new SqlConstantExpression(Expression.Constant(0x7ff0000000000000), null));
        var x0010000000000000 = GetBytes(new SqlConstantExpression(Expression.Constant(0x0010000000000000), null));
     * */

    private readonly ISqlExpressionFactory sqlExpressionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerBinaryTranslator"/> class.
    /// </summary>
    /// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
    /// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
    public SqlServerBinaryTranslator(ISqlExpressionFactory sqlExpressionFactory, IRelationalTypeMappingSource relationalTypeMappingSource)
        : base(sqlExpressionFactory, relationalTypeMappingSource)
    {
        this.sqlExpressionFactory = sqlExpressionFactory;
    }

    /// <inheritdoc/>
    protected override SqlExpression ToValue(SqlExpression sqlExpression, Type type)
        => type == typeof(double)
            ? ToDouble(sqlExpression)
            : base.ToValue(sqlExpression, type);

    /// <summary>
    /// Converts binary(8) to double precision.
    /// </summary>
    /// <param name="sqlExpression">The SqlExpression to convert.</param>
    /// <returns>Sql expression representing the double value.</returns>
    /// <remarks>
    /// Use this method to convert: http://multikoder.blogspot.ca/2013/03/converting-varbinary-to-float-in-t-sql.html.
    /// </remarks>
    private SqlBinaryExpression ToDouble(SqlExpression sqlExpression)
    {
        var colNameBigInt = new SqlUnaryExpression(ExpressionType.Convert, sqlExpression, typeof(long), null);

        var sql1023 = new SqlConstantExpression(Expression.Constant(1023), null);

        // Line 1
        var l1 = sqlExpressionFactory.Function("SIGN", new[] { colNameBigInt }, true, SignArgumentsPropagateNullability, colNameBigInt.Type);

        // Line 2
        var l2e1 = new SqlBinaryExpression(ExpressionType.And, colNameBigInt, X000FFFFFFFFFFFFF, typeof(long), null);
        var l2e2 = new SqlUnaryExpression(ExpressionType.Convert, l2e1, typeof(double), null);
        var l2e3 = new SqlBinaryExpression(ExpressionType.Multiply, l2e2, TwoToThePowerOfMinus52Sql, typeof(double), null);
        var l2 = new SqlBinaryExpression(ExpressionType.Add, OneFloat, l2e3, typeof(double), null);

        // Line 3
        var l3e1 = new SqlBinaryExpression(ExpressionType.And, colNameBigInt, X7ff0000000000000, typeof(long), null);
        var l3e2 = new SqlBinaryExpression(ExpressionType.Divide, l3e1, X0010000000000000, typeof(long), null);
        var l3e3 = new SqlBinaryExpression(ExpressionType.Subtract, l3e2, sql1023, typeof(long), null);
        var l3 = sqlExpressionFactory.Function("POWER", new SqlExpression[] { TwoFloat, l3e3 }, false, PowerArgumentsPropagateNullability, colNameBigInt.Type);

        var l1l2 = new SqlBinaryExpression(ExpressionType.Multiply, l1, l2, typeof(double), null);
        var final = new SqlBinaryExpression(ExpressionType.Multiply, l1l2, l3, typeof(double), null);

        return final;
    }
}