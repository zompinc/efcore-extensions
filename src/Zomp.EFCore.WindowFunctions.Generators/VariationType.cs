namespace Zomp.EFCore.WindowFunctions.Generators;

/// <summary>
/// Which <see cref="ExpressionType"/> overloads a window function gets.
/// </summary>
internal enum VariationType
{
    All,
    StructsOnly,
    Generic,
}
