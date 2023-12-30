namespace Zomp.EFCore.Testing;

public static class LinqExtensions
{
    public static IEnumerable<TR?> LastNonNull<T, TR>(this IEnumerable<T> list, Func<T, TR?> func)
    {
        TR? lastNonnull = default;
        foreach (var elem in list)
        {
            var currentResult = func.Invoke(elem);
            if (currentResult is not null)
            {
                lastNonnull = currentResult;
            }

            yield return lastNonnull;
        }
    }
}
