namespace Zomp.EFCore.WindowFunctions.Sqlite.Query.Internal;

/// <summary>
/// A class that processes the <see cref="SelectExpression" /> including  window functions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsSqliteParameterBasedSqlProcessor"/> class.
/// </remarks>
public class WindowFunctionsSqliteParameterBasedSqlProcessor : SqliteParameterBasedSqlProcessor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqliteParameterBasedSqlProcessor"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="parameters">Processor parameters.</param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "EF Core 8")]
    public WindowFunctionsSqliteParameterBasedSqlProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, RelationalParameterBasedSqlProcessorParameters parameters)
        : base(dependencies, parameters)
    {
    }

    /// <inheritdoc/>
    protected override Expression ProcessSqlNullability(Expression queryExpression, ParametersCacheDecorator parametersDecorator)
        => new WindowFunctionsSqliteSqlNullabilityProcessor(Dependencies, Parameters)
            .Process(queryExpression, parametersDecorator);
}
