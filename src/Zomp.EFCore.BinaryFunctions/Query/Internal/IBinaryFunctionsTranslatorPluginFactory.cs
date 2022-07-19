namespace Zomp.EFCore.BinaryFunctions.Query.Internal;

/// <summary>
/// A factory for creating <see cref="BinaryTranslator" /> instances.
/// </summary>
public interface IBinaryFunctionsTranslatorPluginFactory
{
    /// <summary>
    /// Creates binary functions translator.
    /// </summary>
    /// <returns>Binary functions translator.</returns>
    BinaryTranslator Create();
}