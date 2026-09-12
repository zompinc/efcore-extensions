namespace Zomp.EFCore.WindowFunctions.Generators;

/// <summary>
/// The type of the expression argument of one overload.
/// </summary>
internal enum ExpressionType
{
    NonNullableStruct,
    NullableStruct,
    ByteArray,
    String,
    Generic,
}
