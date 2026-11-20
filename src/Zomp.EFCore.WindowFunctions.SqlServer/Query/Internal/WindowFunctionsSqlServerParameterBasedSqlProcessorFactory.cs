namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// Factory for generating <see cref="WindowFunctionsSqlServerParameterBasedSqlProcessor"/> instances.
/// </summary>
public class WindowFunctionsSqlServerParameterBasedSqlProcessorFactory : SqlServerParameterBasedSqlProcessorFactory
{
    private readonly ISqlServerSingletonOptions sqlServerSingletonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqlServerParameterBasedSqlProcessorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="sqlServerSingletonOptions">The singleton option.</param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "The options also go to the base constructor, which a primary constructor can't capture (CS9107)")]
    public WindowFunctionsSqlServerParameterBasedSqlProcessorFactory(RelationalParameterBasedSqlProcessorDependencies dependencies, ISqlServerSingletonOptions sqlServerSingletonOptions)
        : base(dependencies, sqlServerSingletonOptions)
    {
        this.sqlServerSingletonOptions = sqlServerSingletonOptions;
    }

    /// <inheritdoc/>
    public override RelationalParameterBasedSqlProcessor Create(RelationalParameterBasedSqlProcessorParameters parameters)
        => new WindowFunctionsSqlServerParameterBasedSqlProcessor(Dependencies, parameters, this.sqlServerSingletonOptions);
}
