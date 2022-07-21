namespace Zomp.EFCore.Combined.Testing;
public class CombinedTests
{
    private readonly TestDbContext dbContext;

    public CombinedTests(TestDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public void LastNonNullArithmetic()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            LastNonNull =
            EF.Functions.BinaryCast<long, int>(
                EF.Functions.Max(
                    r.Col1.HasValue ? r.Id * (1L << 32) | r.Col1.Value & uint.MaxValue : (long?)null,
                    EF.Functions.Over().OrderBy(r.Id))),
        });

        var result = query.ToList();

        var expectedSequence = LastNonNull(TestFixture.TestRows, r => r.Col1);
        Assert.Equal(expectedSequence, result.Select(r => r.LastNonNull));
    }

    public void LastNonNull()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            LastNonNull =
            EF.Functions.ToValue<int>(
                EF.Functions.Substring(
                    EF.Functions.Max(
                        EF.Functions.Concat(
                            EF.Functions.GetBytes(r.Id), EF.Functions.GetBytes(r.Col1)),
                        EF.Functions.Over().OrderBy(r.Id)),
                    5,
                    4)),
        });

        var result = query.ToList();

        var expectedSequence = LastNonNull(TestFixture.TestRows, r => r.Col1);
        Assert.Equal(expectedSequence, result.Select(r => r.LastNonNull));
    }

    public void LastNonNullShorthand()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            LastNonNull =
            EF.Functions.ToValue<int>(
                EF.Functions.Max(
                    EF.Functions.Concat(
                        EF.Functions.GetBytes(r.Id), EF.Functions.GetBytes(r.Col1)),
                    EF.Functions.Over().OrderBy(r.Id)),
                5),
        });

        var result = query.ToList();

        var expectedSequence = LastNonNull(TestFixture.TestRows, r => r.Col1);
        Assert.Equal(expectedSequence, result.Select(r => r.LastNonNull));
    }

    private static IEnumerable<TR?> LastNonNull<T, TR>(IEnumerable<T> list, Func<T, TR?> func)
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
