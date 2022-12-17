namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Factory for creation of the <see cref="SubQuerySqlServerQueryableMethodTranslatingExpressionVisitor"/>.
/// </summary>
public sealed class SubQuerySqlServerQueryableMethodTranslatingExpressionVisitorFactory
   : SqlServerQueryableMethodTranslatingExpressionVisitorFactory
{
    private readonly QueryableMethodTranslatingExpressionVisitorDependencies dependencies;
    private readonly RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies;
    private readonly IRelationalTypeMappingSource typeMappingSource;

#pragma warning disable CS1591
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Time.")]
    public SubQuerySqlServerQueryableMethodTranslatingExpressionVisitorFactory(
       QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
       RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
       IRelationalTypeMappingSource typeMappingSource)
        : base(dependencies, relationalDependencies)
    {
        this.dependencies = dependencies;
        this.relationalDependencies = relationalDependencies;
        this.typeMappingSource = typeMappingSource;
    }

    /// <inheritdoc />
    public override QueryableMethodTranslatingExpressionVisitor Create(QueryCompilationContext queryCompilationContext)
    {
        // return base.Create(queryCompilationContext);
        return new SubQuerySqlServerQueryableMethodTranslatingExpressionVisitor(dependencies, relationalDependencies, queryCompilationContext, typeMappingSource);
    }
}
