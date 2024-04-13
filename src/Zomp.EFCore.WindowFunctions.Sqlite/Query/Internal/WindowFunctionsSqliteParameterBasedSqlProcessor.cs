#if !EF_CORE_7 && !EF_CORE_6
namespace Zomp.EFCore.WindowFunctions.Sqlite.Query.Internal;

/// <summary>
/// A class that processes the <see cref="SelectExpression" /> including  window functions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsSqliteParameterBasedSqlProcessor"/> class.
/// </remarks>
/// <param name="dependencies">Service dependencies.</param>
/// <param name="useRelationalNulls">A bool value indicating if relational nulls should be used.</param>
public class WindowFunctionsSqliteParameterBasedSqlProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, bool useRelationalNulls)
    : SqliteParameterBasedSqlProcessor(dependencies, useRelationalNulls)
{
    /// <inheritdoc/>
    protected override Expression ProcessSqlNullability(Expression queryExpression, IReadOnlyDictionary<string, object?> parametersValues, out bool canCache)
        => new WindowFunctionsSqliteSqlNullabilityProcessor(Dependencies, UseRelationalNulls).Process(queryExpression, parametersValues, out canCache);
}
#endif
