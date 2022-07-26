namespace Zomp.EFCore.WindowFunctions.Testing;

public class NullSensitiveComparer<T> : IComparer<T?>
    where T : struct
{
    private readonly bool nullsLast;

    public NullSensitiveComparer(bool nullsLast = false)
    {
        this.nullsLast = nullsLast;
    }

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
