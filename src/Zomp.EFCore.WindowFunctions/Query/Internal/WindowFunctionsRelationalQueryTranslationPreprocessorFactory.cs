namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// The WindowFunctionsRelationalQueryTranslationPreprocessorFactory.
/// </summary>
/// <param name="dependencies">The Type Mapping Source Dependencies.</param>
/// <param name="relationalDependencies">Relational Type Mapping Source Dependencies.</param>
public class WindowFunctionsRelationalQueryTranslationPreprocessorFactory(QueryTranslationPreprocessorDependencies dependencies, RelationalQueryTranslationPreprocessorDependencies relationalDependencies)
    : RelationalQueryTranslationPreprocessorFactory(dependencies, relationalDependencies)
{
    /// <inheritdoc/>
    public override QueryTranslationPreprocessor Create(QueryCompilationContext queryCompilationContext)
        => new WindowFunctionsRelationalQueryTranslationPreprocessor(Dependencies, RelationalDependencies, queryCompilationContext);
}