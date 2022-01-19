namespace Zomp.EFCore.WindowFunctions.Testing;

public class MaxTests
{
    private readonly TestDbContext dbContext;

    public MaxTests(TestDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public void SimpleMax()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.Id, EF.Functions.Over()),
        });

        var result = query.ToList();

        var maxId = TestFixture.TestRows.Max(r => r.Id);
        var expectedSequence = Enumerable.Range(0, TestFixture.TestRows.Length).Select(_ => (int?)maxId);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    public void SimpleMaxNullable()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.Col1, EF.Functions.Over()),
        });

        var result = query.ToList();

        var maxId = TestFixture.TestRows.Max(r => r.Col1);
        var expectedSequence = Enumerable.Range(0, TestFixture.TestRows.Length).Select(_ => maxId);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    public void MaxBetweenCurrentRowAndOne()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.Id, EF.Functions.Over().OrderBy(r.Id).Rows().FromCurrentRow().ToFollowing(1)),
        });

        var result = query.ToList();

        var maxId = TestFixture.TestRows.Max(r => r.Id);
        var expectedSequence = TestFixture.TestRows
            .Select((_, i)
            => (int?)(i < TestFixture.TestRows.Length - 1
            ? Math.Max(TestFixture.TestRows[i].Id, TestFixture.TestRows[i + 1].Id)
            : TestFixture.TestRows[i].Id));
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    public void MaxBetweenTwoPreceding()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.Id, EF.Functions.Over().OrderBy(r.Id).Rows().FromPreceding(2).ToPreceding(1)),
        });

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .Select((_, i)
            => i == 0 ? (int?)null
            : i == 1 ? TestFixture.TestRows[0].Id
            : Math.Max(TestFixture.TestRows[i - 2].Id, TestFixture.TestRows[i - 1].Id));
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    public void MaxBetweenTwoFollowing()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.Id, EF.Functions.Over().OrderBy(r.Id).Rows().FromFollowing(1).ToFollowing(2)),
        });

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows
            .Select((_, i)
            => i < TestFixture.TestRows.Length - 2
            ? Math.Max(TestFixture.TestRows[i + 1].Id, TestFixture.TestRows[i + 2].Id)
            : i < TestFixture.TestRows.Length - 1 ? TestFixture.TestRows[i + 1].Id : (int?)null);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    public void MaxBetweenFollowingAndUnbounded()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Original = r,
            Max = EF.Functions.Max(r.Id, EF.Functions.Over().OrderBy(r.Id).Rows().FromFollowing(1).ToUnbounded()),
        })
        .OrderBy(r => r.Original.Id);

        var result = query.ToList();

        var maxId = TestFixture.TestRows.Max(r => r.Id);
        var expectedSequence = TestFixture.TestRows
            .Select((_, i)
            => i < TestFixture.TestRows.Length - 1 ? maxId : (int?)null);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    public void MaxWithPartition()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(
                r.Id,
                EF.Functions.Over().PartitionBy(r.Id / 10)),
        });

        var result = query.ToList();

        var groups = TestFixture.TestRows.GroupBy(r => r.Id / 10)
            .ToDictionary(r => r.Key, r => r.Max(s => s.Id));

        var expectedSequence = TestFixture.TestRows.Select(r => (int?)groups[r.Id / 10]);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    public void MaxWith2Partitions()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Original = r,
            Max = EF.Functions.Max(
                r.Id,
                EF.Functions.Over().PartitionBy(r.Id / 10).ThenBy(r.Date.DayOfYear % 2)),
        })
        .OrderBy(r => r.Original.Id);

        var result = query.ToList();

        var groups = TestFixture.TestRows.GroupBy(z => (z.Id / 10, z.Date.DayOfYear % 2))
            .ToDictionary(r => r.Key, r => r.Max(s => (int?)s.Id));

        var expectedSequence = TestFixture.TestRows.Select(r => groups[(r.Id / 10, r.Date.DayOfYear % 2)]);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    public void SimpleMaxWithCast()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max((long)r.Id, EF.Functions.Over()),
        });

        var result = query.ToList();

        var maxId = TestFixture.TestRows.Max(r => r.Id);
        var expectedSequence = Enumerable.Range(0, TestFixture.TestRows.Length).Select(_ => (long?)maxId);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    public void MaxWithCastToString()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.Col1.ToString(), EF.Functions.Over()),
        });

        var result = query.ToList();

        var max = TestFixture.TestRows.Max(r => r.Col1?.ToString(CultureInfo.InvariantCulture));
        var expectedSequence = Enumerable.Range(0, TestFixture.TestRows.Length).Select(_ => max);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
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