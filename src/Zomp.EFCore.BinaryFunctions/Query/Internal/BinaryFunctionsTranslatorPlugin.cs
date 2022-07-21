namespace Zomp.EFCore.BinaryFunctions.Query.Internal;

/// <summary>
/// Binary Functions Translator Plugin.
/// </summary>
public class BinaryFunctionsTranslatorPlugin : IMethodCallTranslatorPlugin
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryFunctionsTranslatorPlugin"/> class.
    /// </summary>
    /// <param name="binaryTranslatorPluginFactory">Binary Translator Plugin Factory.</param>
    public BinaryFunctionsTranslatorPlugin(IBinaryTranslatorPluginFactory binaryTranslatorPluginFactory)
    {
        var list = new List<IMethodCallTranslator>
        {
            binaryTranslatorPluginFactory.Create(),
        };
        Translators = list;
    }

    /// <inheritdoc/>
    public IEnumerable<IMethodCallTranslator> Translators { get; }
}