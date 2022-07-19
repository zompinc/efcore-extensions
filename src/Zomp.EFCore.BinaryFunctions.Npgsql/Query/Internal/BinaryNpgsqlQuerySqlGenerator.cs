namespace Zomp.EFCore.BinaryFunctions.Npgsql.Query.Internal;

/// <summary>
/// Query SQL generator for Npgsql which includes binary operations.
/// </summary>
public class BinaryNpgsqlQuerySqlGenerator : NpgsqlQuerySqlGenerator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryNpgsqlQuerySqlGenerator"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="reverseNullOrderingEnabled">Null Ordering.</param>
    /// <param name="postgresVersion">Postgres Version.</param>
    public BinaryNpgsqlQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies, bool reverseNullOrderingEnabled, Version postgresVersion)
        : base(dependencies, reverseNullOrderingEnabled, postgresVersion)
    {
    }

    /// <inheritdoc/>
    protected override string GetOperator(SqlBinaryExpression e)
        => e.OperatorType switch
        {
            ExpressionType.Add when
                e.Type == typeof(BitArray) || e.Left.TypeMapping?.ClrType == typeof(BitArray) || e.Right.TypeMapping?.ClrType == typeof(BitArray) ||
                e.Type == typeof(byte[]) || e.Left.TypeMapping?.ClrType == typeof(byte[]) || e.Right.TypeMapping?.ClrType == typeof(byte[])
                => " || ",
            _ => base.GetOperator(e),
        };
}