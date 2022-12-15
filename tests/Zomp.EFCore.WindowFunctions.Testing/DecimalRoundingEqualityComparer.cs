using System.Diagnostics.CodeAnalysis;

namespace Zomp.EFCore.WindowFunctions.Testing;

public class DecimalRoundingEqualityComparer : IEqualityComparer<decimal?>
{
    private readonly decimal epsilon;

    public DecimalRoundingEqualityComparer(int roundingDecimals)
    {
        epsilon = (decimal)Math.Pow(0.1, roundingDecimals);
    }

    public bool Equals(decimal? x, decimal? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return Math.Abs(x.Value - y.Value) < this.epsilon;
    }

    public int GetHashCode([DisallowNull] decimal? obj)
    {
        return obj.GetHashCode();
    }
}
