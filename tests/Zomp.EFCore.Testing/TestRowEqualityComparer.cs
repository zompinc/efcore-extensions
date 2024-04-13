namespace Zomp.EFCore.Testing;

public class TestRowEqualityComparer : IEqualityComparer<TestRow>
{
    public static TestRowEqualityComparer Default { get; } = new();

    public bool Equals(TestRow? x, TestRow? y) => x is null || y is null
            ? x is null && y is null
            : x.Id == y.Id
            && x.Col1 == y.Col1
            && x.SomeGuid == y.SomeGuid
            && x.Date == y.Date
            && x.IdBytes.SequenceEqual(y.IdBytes);

    public int GetHashCode([DisallowNull] TestRow obj) => HashCode.Combine(obj.Id.GetHashCode(), obj.Col1.GetHashCode(), obj.SomeGuid.GetHashCode(), obj.Date.GetHashCode());
}