namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Extends the capabilities of <see cref="RelationalQueryableMethodTranslatingExpressionVisitor"/>.
/// </summary>
[SuppressMessage("Usage", "EF1001", Justification = "Victor says", MessageId = "Internal EF Core API usage.")]
public class SubQuerySqlServerQueryableMethodTranslatingExpressionVisitor
   : Microsoft.EntityFrameworkCore.SqlServer.Query.Internal.SqlServerQueryableMethodTranslatingExpressionVisitor
{
    private readonly IRelationalTypeMappingSource typeMappingSource;

    #pragma warning disable CS1591
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Time.")]
    public SubQuerySqlServerQueryableMethodTranslatingExpressionVisitor(
       QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
       RelationalQueryableMethodTranslatingExpressionVisitorDependencies relationalDependencies,
       QueryCompilationContext queryCompilationContext,
       IRelationalTypeMappingSource typeMappingSource)
       : base(dependencies, relationalDependencies, queryCompilationContext)
    {
        this.typeMappingSource = typeMappingSource ?? throw new ArgumentNullException(nameof(typeMappingSource));
    }

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Time.")]
    protected SubQuerySqlServerQueryableMethodTranslatingExpressionVisitor(
       SubQuerySqlServerQueryableMethodTranslatingExpressionVisitor parentVisitor,
       IRelationalTypeMappingSource typeMappingSource)
       : base(parentVisitor)
    {
        this.typeMappingSource = typeMappingSource ?? throw new ArgumentNullException(nameof(typeMappingSource));
    }

    /// <inheritdoc />
    protected override QueryableMethodTranslatingExpressionVisitor CreateSubqueryVisitor()
    {
        return new SubQuerySqlServerQueryableMethodTranslatingExpressionVisitor(this, typeMappingSource);
    }

    /// <inheritdoc />
    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
    {
        return this.TranslateRelationalMethods(methodCallExpression) ??
               base.VisitMethodCall(methodCallExpression);
    }
}
