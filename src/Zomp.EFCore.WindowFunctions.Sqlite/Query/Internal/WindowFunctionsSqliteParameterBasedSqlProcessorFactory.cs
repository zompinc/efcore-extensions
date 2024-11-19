namespace Zomp.EFCore.WindowFunctions.Sqlite.Query.Internal;

/// <summary>
/// Factory for producing <see cref="WindowFunctionsSqliteParameterBasedSqlProcessor"/> instances.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsSqliteParameterBasedSqlProcessorFactory"/> class.
/// </remarks>
/// <param name="dependencies">Relational Parameter Based Sql ProcessorDependencies.</param>
public class WindowFunctionsSqliteParameterBasedSqlProcessorFactory(RelationalParameterBasedSqlProcessorDependencies dependencies)
    : SqliteParameterBasedSqlProcessorFactory(dependencies)
{
    private readonly RelationalParameterBasedSqlProcessorDependencies dependencies = dependencies;

    /// <inheritdoc/>
    public override RelationalParameterBasedSqlProcessor Create(bool useRelationalNulls)
        => new WindowFunctionsSqliteParameterBasedSqlProcessor(dependencies, useRelationalNulls);
}
