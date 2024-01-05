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

    [SuppressMessage("Design", "CA1065:Do not raise exceptions in unexpected locations", Justification = "Testing code")]
    public int GetHashCode([DisallowNull] decimal? obj)
    {
        throw new NotImplementedException();
    }
}
