namespace Zomp.EFCore.WindowFunctions.Testing;

public class NullSensitiveComparer<T>(bool nullsLast = false) : IComparer<T?>
    where T : struct
{
    public int Compare(T? x, T? y)
    {
        if (x is null && y is null)
        {
            return 0;
        }
        else if (x is null)
        {
            return nullsLast ? 1 : -1;
        }
        else if (y is null)
        {
            return nullsLast ? -1 : 1;
        }

        return Comparer<T>.Default.Compare(x.Value, y.Value);
    }
}
