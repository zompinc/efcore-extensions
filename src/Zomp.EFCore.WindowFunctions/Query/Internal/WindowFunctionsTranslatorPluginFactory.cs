namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Factory for WindowFunctionsTranslatorPlugin.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WindowFunctionsTranslatorPluginFactory"/> class.
/// </remarks>
/// <param name="sqlExpressionFactory">Instance of sql expression factory.</param>
public class WindowFunctionsTranslatorPluginFactory(ISqlExpressionFactory sqlExpressionFactory)
    : IWindowFunctionsTranslatorPluginFactory
{
    private readonly ISqlExpressionFactory sqlExpressionFactory = sqlExpressionFactory;

    /// <inheritdoc/>
    public virtual WindowFunctionsTranslator Create() => new(sqlExpressionFactory);
}