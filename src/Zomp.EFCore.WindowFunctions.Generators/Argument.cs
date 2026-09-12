namespace Zomp.EFCore.WindowFunctions.Generators;

/// <summary>
/// A parameter of a generated method.
/// </summary>
internal sealed class Argument(string type, string name, string description)
{
    public string ToParameterDeclaration() => $"{type} {name}";

    public string ToTrivia() => $"    /// <param name=\"{name.Trim('@')}\">{description}</param>";
}
