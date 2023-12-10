#if !EF_CORE_7 && !EF_CORE_6
namespace Zomp.EFCore.WindowFunctions.Sqlite.Query.Internal;

/// <summary>
/// Factory for producing <see cref="WindowFunctionsSqliteParameterBasedSqlProcessor"/> instances.
/// </summary>
public class WindowFunctionsSqliteParameterBasedSqlProcessorFactory : SqliteParameterBasedSqlProcessorFactory
{
    private readonly RelationalParameterBasedSqlProcessorDependencies dependencies;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqliteParameterBasedSqlProcessorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Relational Parameter Based Sql ProcessorDependencies.</param>
    public WindowFunctionsSqliteParameterBasedSqlProcessorFactory(RelationalParameterBasedSqlProcessorDependencies dependencies)
        : base(dependencies)
    {
        this.dependencies = dependencies;
    }

    /// <inheritdoc/>
    public override RelationalParameterBasedSqlProcessor Create(bool useRelationalNulls)
        => new WindowFunctionsSqliteParameterBasedSqlProcessor(dependencies, useRelationalNulls);
}
#endif
