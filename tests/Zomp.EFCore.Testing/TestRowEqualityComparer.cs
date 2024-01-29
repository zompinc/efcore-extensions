namespace Zomp.EFCore.Testing;

public class TestRowEqualityComparer : IEqualityComparer<TestRow>
{
    public static TestRowEqualityComparer Default { get; } = new();

    public bool Equals(TestRow? x, TestRow? y)
    {
        if (x is null || y is null)
        {
            return x is null && y is null;
        }

        return x.Id == y.Id
            && x.Col1 == y.Col1
            && x.SomeGuid == y.SomeGuid
            && x.Date == y.Date
            && x.IdBytes.SequenceEqual(y.IdBytes);
    }

    public int GetHashCode([DisallowNull] TestRow obj)
    {
        return HashCode.Combine(obj.Id.GetHashCode(), obj.Col1.GetHashCode(), obj.SomeGuid.GetHashCode(), obj.Date.GetHashCode());
    }
}