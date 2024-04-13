using System.Diagnostics.CodeAnalysis;

namespace Zomp.EFCore.WindowFunctions.Testing;

public class DecimalRoundingEqualityComparer(int roundingDecimals) : IEqualityComparer<decimal?>
{
    private readonly decimal epsilon = (decimal)Math.Pow(0.1, roundingDecimals);

    public bool Equals(decimal? x, decimal? y)
    {
        return (x == null && y == null) || (x is not null && y is not null && Math.Abs(x.Value - y.Value) < epsilon);
    }

    [SuppressMessage("Design", "CA1065:Do not raise exceptions in unexpected locations", Justification = "Testing code")]
    public int GetHashCode([DisallowNull] decimal? obj) => throw new NotImplementedException();
}
