namespace Zomp.EFCore.WindowFunctions.Testing;

public abstract partial class CountTests<TResult>
    where TResult : IConvertible
{
    [Fact]
    public void CountBasic()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<int, TResult>(r.Id, EF.Functions.Over()),
        });

        var result = query.ToList();

        var maxId = TestRows.Length;
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => maxId);
        Assert.Equal(expectedSequence, result.Select(r => r.Count.ToInt32(null)));
    }

    [Fact]
    public void CountBasicNullable()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<int?, TResult>(r.Col1, EF.Functions.Over()),
        });

        var result = query.ToList();

        var countId = TestRows.Count(x => x.Col1 is not null);
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => countId);
        Assert.Equal(expectedSequence, result.Select(r => r.Count.ToInt32(null)));
    }

    [Fact]
    public void CountBetweenCurrentRowAndNext()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<int, TResult>(r.Id, EF.Functions.Over().OrderBy(r.Id).Rows().FromCurrentRow().ToFollowing(1)),
        });

        var result = query.ToList();

        var expectedSequence = TestRows
            .Select((_, i) => i < TestRows.Length - 1 ? 2 : 1);
        Assert.Equal(expectedSequence, result.Select(r => r.Count.ToInt32(null)));
    }

    [Fact]
    public void CountBetweenCurrentRowAndNextNullable()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<int?, TResult>(r.Col1, EF.Functions.Over().OrderBy(r.Id).Rows().FromCurrentRow().ToFollowing(1)),
        });

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i)
            => TestRows.CountNonNulls(z => z.Col1, i, i + 1));
        Assert.Equal(expectedSequence, result.Select(r => r.Count.ToInt32(null)));
    }

    [Fact]
    public void CountBetweenTwoPreceding()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<int?, TResult>(r.Col1, EF.Functions.Over().OrderBy(r.Id).Rows().FromPreceding(2).ToPreceding(1)),
        });

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i)
            => TestRows.CountNonNulls(z => z.Col1, i - 2, i - 1));
        Assert.Equal(expectedSequence, result.Select(r => r.Count.ToInt32(null)));
    }

    [Fact]
    public void CountBetweenTwoFollowing()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<int?, TResult>(r.Col1, EF.Functions.Over().OrderBy(r.Id).Rows().FromFollowing(1).ToFollowing(2)),
        });

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i)
            => TestRows.CountNonNulls(z => z.Col1, i + 1, i + 2));
        Assert.Equal(expectedSequence, result.Select(r => r.Count.ToInt32(null)));
    }

    [Fact]
    public void CountBetweenFollowingAndUnbounded()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Original = r,
            Count = EF.Functions.Count<int?, TResult>(r.Col1, EF.Functions.Over().OrderBy(r.Id).Rows().FromFollowing(1).ToUnbounded()),
        })
        .OrderBy(r => r.Original.Id);

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i)
            => TestRows.CountNonNulls(z => z.Col1, i + 1, TestRows.Length - 1));
        Assert.Equal(expectedSequence, result.Select(r => r.Count.ToInt32(null)));
    }

    [Fact]
    public void CountWithPartition()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<int?, TResult>(
                r.Col1,
                EF.Functions.Over().PartitionBy(r.Id / 10)),
        });

        var result = query.ToList();

        var groups = TestRows.GroupBy(r => r.Id / 10)
            .ToDictionary(
            r => r.Key,
            r => r.Count(z => z.Col1 is not null));

        var expectedSequence = TestRows.Select(r => groups[r.Id / 10]);
        Assert.Equal(expectedSequence, result.Select(r => r.Count.ToInt32(null)));
    }

    [Fact]
    public void CountWith2Partitions()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Original = r,
            Count = EF.Functions.Count<int, TResult>(
                r.Id,
                EF.Functions.Over().PartitionBy(r.Id / 10).ThenBy(r.Date.DayOfYear % 2)),
        })
        .OrderBy(r => r.Original.Id);

        var result = query.ToList();

        var groups = TestRows.GroupBy(z => (z.Id / 10, z.Date.DayOfYear % 2))
            .ToDictionary(
            r => r.Key,
            r => r.Count());

        var expectedSequence = TestRows.Select(r => groups[(r.Id / 10, r.Date.DayOfYear % 2)]);
        Assert.Equal(expectedSequence, result.Select(r => r.Count.ToInt32(null)));
    }

    [SkippableFact]
    public void SimpleCountWithCast()
    {
        Skip.If(DbContext.IsSqlite, "Look more into this");

        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<long, TResult>(r.Id, EF.Functions.Over()),
        });

        var result = query.ToList();

        var count = TestRows.Length;
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => count);
        Assert.Equal(expectedSequence, result.Select(r => r.Count.ToInt32(null)));
    }

    [SkippableFact]
    public void CountWithCastToString()
    {
        Skip.If(DbContext.IsSqlite, "Look more into this");

        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<string, TResult>(r.Col1.ToString(), EF.Functions.Over()),
        });

        var result = query.ToList();

        var count = TestRows.Count(r => r.Col1?.ToString(CultureInfo.InvariantCulture) != null);
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => count);
        Assert.Equal(expectedSequence, result.Select(r => r.Count.ToInt32(null)));
    }

    [SkippableFact]
    public void CountBinary()
    {
        Skip.If(DbContext.IsSqlite, "Look more into this");

        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<byte[]?, TResult>(r.IdBytes, EF.Functions.Over()),
        });

        var result = query.ToList();

        var count = TestRows.Length;
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => count);

        Assert.Equal(expectedSequence, result.Select(r => r.Count.ToInt32(null)));
    }
}