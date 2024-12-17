namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// The WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitorFactory.
/// </summary>
public class WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitorFactory : NpgsqlQueryableMethodTranslatingExpressionVisitorFactory
{
    private readonly QueryableMethodTranslatingExpressionVisitorDependencies dependencies;
    private readonly RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies;
#if !EF_CORE_8
    private readonly INpgsqlSingletonOptions npgsqlSingletonOptions;
#endif

#if !EF_CORE_8
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Type mapping source dependencies.</param>
    /// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
    /// <param name="npgsqlSingletonOptions">NpgSql Singleton Options.</param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "EF Core 8")]
    public WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitorFactory(
        QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
        RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
        INpgsqlSingletonOptions npgsqlSingletonOptions)
        : base(dependencies, relationalDependencies, npgsqlSingletonOptions)
    {
        this.dependencies = dependencies;
        this.relationalDependencies = relationalDependencies;
        this.npgsqlSingletonOptions = npgsqlSingletonOptions;
    }
#else
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Type mapping source dependencies.</param>
    /// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "EF Core 8")]
    public WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitorFactory(QueryableMethodTranslatingExpressionVisitorDependencies dependencies, RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
    {
        this.dependencies = dependencies;
        this.relationalDependencies = relationalDependencies;
    }
#endif

#if !EF_CORE_8
    /// <inheritdoc/>
    public override QueryableMethodTranslatingExpressionVisitor Create(QueryCompilationContext queryCompilationContext)
        => new WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitor(dependencies, relationalDependencies, (RelationalQueryCompilationContext)queryCompilationContext, npgsqlSingletonOptions);
#else
    /// <inheritdoc/>
    public override QueryableMethodTranslatingExpressionVisitor Create(QueryCompilationContext queryCompilationContext)
        => new WindowFunctionsNpgsqlQueryableMethodTranslatingExpressionVisitor(dependencies, relationalDependencies, queryCompilationContext);
#endif
}