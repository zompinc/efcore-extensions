namespace Zomp.EFCore.BinaryFunctions.Query.Internal;

/// <summary>
/// A factory for creating <see cref="BinaryTranslator" /> instances.
/// </summary>
public interface IBinaryTranslatorPluginFactory
{
    /// <summary>
    /// Creates binary translator.
    /// </summary>
    /// <returns>The binary translator.</returns>
    BinaryTranslator Create();
}