namespace Zomp.EFCore.WindowFunctions.Generators;

/// <summary>
/// One generated overload of a window function.
/// </summary>
internal sealed class Configuration(
    FunctionType functionType,
    ExpressionType expressionType,
    bool customReturnType,
    string? specificReturnType,
    bool respectIgnoreNullsOption,
    int numberOfOptionalArguments)
{
    private static readonly Argument Offset = new("long", "offset", "The offset.");
    private static readonly Argument N = new("long", "n", "The row of the window frame to return the value from, counting from 1.");
    private static readonly Argument NullHandling = new("NullHandling?", "nullHandling", "Respect nulls or ignore nulls. If omitted or <see langword=\"null\" /> is specified, provider's default is used which is to respect nulls.");

    public bool CustomReturnType => customReturnType;

    public bool IsGeneric => expressionType is ExpressionType.NonNullableStruct or ExpressionType.NullableStruct or ExpressionType.Generic;

    public bool IsGenericWithQualifier => expressionType is ExpressionType.NonNullableStruct or ExpressionType.NullableStruct;

    public string ReturnType => customReturnType ? "TResult" : specificReturnType is { Length: > 0 } ? specificReturnType : IsGeneric ? "T" : TypeDefinition;

    public string GenericTypeParameters => functionType != FunctionType.NoArguments && IsGeneric ? customReturnType ? "<T, TResult>" : "<T>" : string.Empty;

    public string Arguments => string.Concat(ArgList.Select(a => a.ToParameterDeclaration() + ", "));

    public IEnumerable<string> ParameterTrivia => ArgList.Select(a => a.ToTrivia());

    private string TypeDefinition => GetTypeDefinition(expressionType);

    private string AlwaysNullableTypeDefinition => expressionType switch
    {
        ExpressionType.NullableStruct or ExpressionType.Generic => TypeDefinition,
        ExpressionType.NonNullableStruct or ExpressionType.ByteArray or ExpressionType.String => $"{TypeDefinition}?",
        _ => throw new InvalidOperationException($"Unknown expression type {expressionType}."),
    };

    private string ArgumentType => IsGeneric ? TypeDefinition : TypeDefinition + "?";

    private Argument MainExpression => new(ArgumentType, "expression", "Expression to run window function on.");

    private Argument DefaultExpression => new(AlwaysNullableTypeDefinition, "@default", "The default.");

    private List<Argument> ArgList => functionType switch
    {
        FunctionType.NoArguments => [],
        FunctionType.SingleArgument => [MainExpression],
        FunctionType.LeadLag => GetLeadLagArguments(),
        FunctionType.NthValue => [MainExpression, N],
        _ => throw new InvalidOperationException($"Unknown function type {functionType}."),
    };

    private static string GetTypeDefinition(ExpressionType expressionType) => expressionType switch
    {
        ExpressionType.NonNullableStruct => "T",
        ExpressionType.NullableStruct or ExpressionType.Generic => "T?",
        ExpressionType.ByteArray => "byte[]",
        ExpressionType.String => "string",
        _ => throw new ArgumentOutOfRangeException(nameof(expressionType)),
    };

    private List<Argument> GetLeadLagArguments()
    {
        List<Argument> list = [MainExpression, Offset, DefaultExpression];
        list.RemoveRange(list.Count - numberOfOptionalArguments, numberOfOptionalArguments);
        if (respectIgnoreNullsOption)
        {
            list.Add(NullHandling);
        }

        return list;
    }
}
