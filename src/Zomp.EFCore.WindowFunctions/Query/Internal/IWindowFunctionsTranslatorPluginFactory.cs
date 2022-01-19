namespace Zomp.EFCore.WindowFunctions.Query.Internal;

/// <summary>
/// A factory for creating <see cref="WindowFunctionsTranslator" /> instances.
/// </summary>
public interface IWindowFunctionsTranslatorPluginFactory
{
    /// <summary>
    /// Creates Window functions translator.
    /// </summary>
    /// <returns>Window functions translator.</returns>
    WindowFunctionsTranslator Create();
}