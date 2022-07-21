namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// Window Functions Translator Plugin.
/// </summary>
public class WindowFunctionsTranslatorPlugin : IMethodCallTranslatorPlugin
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsTranslatorPlugin"/> class.
    /// </summary>
    /// <param name="windowFunctionsTranslatorPluginFactory">Window Functions Translator Plugin Factory.</param>
    public WindowFunctionsTranslatorPlugin(IWindowFunctionsTranslatorPluginFactory windowFunctionsTranslatorPluginFactory)
    {
        var list = new List<IMethodCallTranslator>
        {
            windowFunctionsTranslatorPluginFactory.Create(),
        };
        Translators = list;
    }

    /// <inheritdoc/>
    public IEnumerable<IMethodCallTranslator> Translators { get; }
}