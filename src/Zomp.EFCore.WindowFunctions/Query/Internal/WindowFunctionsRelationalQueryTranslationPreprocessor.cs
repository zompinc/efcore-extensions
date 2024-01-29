namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// The WindowFunctionsRelationalQueryTranslationPreprocessor.
/// </summary>
/// <param name="dependencies">Type mapping source dependencies.</param>
/// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
/// <param name="queryCompilationContext">The query compilation context object to use.</param>
public class WindowFunctionsRelationalQueryTranslationPreprocessor(QueryTranslationPreprocessorDependencies dependencies, RelationalQueryTranslationPreprocessorDependencies relationalDependencies, QueryCompilationContext queryCompilationContext) : RelationalQueryTranslationPreprocessor(dependencies, relationalDependencies, queryCompilationContext)
{
    /// <inheritdoc/>
    public override Expression Process(Expression query)
    {
        var @base = base.Process(query);
        var wfd = new WindowFunctionDetector();
        var rewritten = wfd.Visit(@base);
        return rewritten;
    }
}
