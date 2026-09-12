namespace Zomp.EFCore.WindowFunctions.Generators;

/// <summary>
/// A window function and the shape of the overloads generated for it.
/// </summary>
internal sealed class FunctionDefinition(
    string name,
    string summary,
    FunctionType functionType = FunctionType.SingleArgument,
    VariationType variationType = VariationType.All,
    bool customReturnType = false,
    bool nonNullableReturnType = false,
    string? specificReturnType = null)
{
    public string Name { get; } = name;

    public string Summary { get; } = summary;

    public FunctionType FunctionType { get; } = functionType;

    public VariationType VariationType { get; } = variationType;

    /// <summary>
    /// Gets a value indicating whether to also generate overloads with a <c>TResult</c> return type.
    /// </summary>
    public bool CustomReturnType { get; } = customReturnType;

    public bool NonNullableReturnType { get; } = nonNullableReturnType;

    /// <summary>
    /// Gets the fixed return type, or <see langword="null"/> to derive it from the argument.
    /// </summary>
    public string? SpecificReturnType { get; } = specificReturnType;
}
