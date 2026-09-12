namespace Zomp.EFCore.WindowFunctions.Generators;

/// <summary>
/// The argument list a window function takes before the over clause.
/// </summary>
internal enum FunctionType
{
    NoArguments,
    SingleArgument,
    LeadLag,
}
