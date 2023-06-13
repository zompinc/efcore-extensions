namespace Zomp.EFCore.WindowFunctions.Testing;

public static class ArrayExtensions
{
    public static int CountNonNulls<T, TZ>(this IList<T> list, Func<T, TZ> func, int startIndex, int endIndex)
    {
        static int IsNullAt(IList<T> list, int index, Func<T, TZ> func) =>
            index >= 0 && index < list.Count && func(list[index]) is not null ? 1 : 0;

        var sum = 0;
        for (var i = startIndex; i <= endIndex; ++i)
        {
            sum += IsNullAt(list, i, func);
        }

        return sum;
    }
}
