namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// A SQL translator for window functions in SQL Server.
/// </summary>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
public class SqlServerWindowFunctionsTranslator(ISqlExpressionFactory sqlExpressionFactory)
    : WindowFunctionsTranslator(sqlExpressionFactory)
{
    /// <inheritdoc/>
    protected override SqlExpression Parse(IReadOnlyList<SqlExpression> arguments, string functionName, Type? resultType)
        => base.Parse(arguments, ToSqlServerName(functionName), resultType);

    private static string ToSqlServerName(string functionName) => functionName switch
    {
        "STDDEV_SAMP" => "STDEV",
        "STDDEV_POP" => "STDEVP",
        "VAR_SAMP" => "VAR",
        "VAR_POP" => "VARP",
        "NTH_VALUE" => throw new InvalidOperationException($"SQL Server has no NTH_VALUE function, so {nameof(DbFunctionsExtensions.NthValue)} cannot be translated."),
        _ => functionName,
    };
}