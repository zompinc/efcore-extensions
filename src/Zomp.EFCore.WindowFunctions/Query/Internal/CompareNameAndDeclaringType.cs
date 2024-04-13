namespace Zomp.EFCore.WindowFunctions.Query.Internal;

internal sealed class CompareNameAndDeclaringType : IEqualityComparer<MethodInfo>
{
    public static CompareNameAndDeclaringType Default { get; } = new();

    public bool Equals(MethodInfo? x, MethodInfo? y) => x is null || y is null
            ? x is null && y is null
            : x.Name.Equals(y.Name, StringComparison.Ordinal) && x.DeclaringType == y.DeclaringType;

    public int GetHashCode(MethodInfo method) => HashCode.Combine(method.Name.GetHashCode(StringComparison.Ordinal), method.DeclaringType?.GetHashCode());
}
