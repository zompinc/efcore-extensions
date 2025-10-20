namespace Zomp.EFCore.WindowFunctions.SqlServer.Query.Internal;

/// <summary>
/// Factory for generating <see cref="WindowFunctionsSqlServerParameterBasedSqlProcessor"/> instances.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsSqlServerParameterBasedSqlProcessorFactory"/> class.
/// </remarks>
public class WindowFunctionsSqlServerParameterBasedSqlProcessorFactory : SqlServerParameterBasedSqlProcessorFactory
{
    private readonly ISqlServerSingletonOptions sqlServerSingletonOptions;
#if !EF_CORE_8 && !EF_CORE_9
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqlServerParameterBasedSqlProcessorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="sqlServerSingletonOptions">The singleton option.</param>
    public WindowFunctionsSqlServerParameterBasedSqlProcessorFactory(RelationalParameterBasedSqlProcessorDependencies dependencies, ISqlServerSingletonOptions sqlServerSingletonOptions)
        : base(dependencies, sqlServerSingletonOptions)
    {
        this.sqlServerSingletonOptions = sqlServerSingletonOptions;
    }
#else
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsSqlServerParameterBasedSqlProcessorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    public WindowFunctionsSqlServerParameterBasedSqlProcessorFactory(RelationalParameterBasedSqlProcessorDependencies dependencies)
        : base(dependencies)
    {
    }
#endif

#if !EF_CORE_8 && !EF_CORE_9
    /// <inheritdoc/>
    public override RelationalParameterBasedSqlProcessor Create(RelationalParameterBasedSqlProcessorParameters parameters)
        => new WindowFunctionsSqlServerParameterBasedSqlProcessor(Dependencies, parameters, this.sqlServerSingletonOptions);
#elif !EF_CORE_8 && EF_CORE_9
    /// <inheritdoc/>
    public override RelationalParameterBasedSqlProcessor Create(RelationalParameterBasedSqlProcessorParameters parameters)
        => new WindowFunctionsSqlServerParameterBasedSqlProcessor(Dependencies, parameters);
#else
    /// <inheritdoc/>
    public override RelationalParameterBasedSqlProcessor Create(bool useRelationalNulls)
        => new WindowFunctionsSqlServerParameterBasedSqlProcessor(Dependencies, useRelationalNulls);
#endif
}