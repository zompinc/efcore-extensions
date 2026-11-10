namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// A class that processes the <see cref="SelectExpression" /> including  window functions.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Multiple versions")]
public class WindowFunctionsSqlServerParameterBasedSqlProcessor : SqlServerParameterBasedSqlProcessor
{
    private readonly ISqlServerSingletonOptions sqlServerSingletonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqlServerParameterBasedSqlProcessor"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="parameters">Processor parameters.</param>
    /// <param name="sqlServerSingletonOptions">The singleton option.</param>
    public WindowFunctionsSqlServerParameterBasedSqlProcessor(RelationalParameterBasedSqlProcessorDependencies dependencies, RelationalParameterBasedSqlProcessorParameters parameters, ISqlServerSingletonOptions sqlServerSingletonOptions)
        : base(dependencies, parameters, sqlServerSingletonOptions)
    {
        this.sqlServerSingletonOptions = sqlServerSingletonOptions;
    }

    /// <inheritdoc/>
    protected override Expression ProcessSqlNullability(Expression selectExpression, ParametersCacheDecorator Decorator)
        => new WindowFunctionsSqlServerSqlNullabilityProcessor(Dependencies, Parameters, sqlServerSingletonOptions).Process(
            selectExpression, Decorator);
}