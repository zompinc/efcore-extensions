using Zomp.EFCore.WindowFunctions.Testing;
using static Zomp.EFCore.Testing.TestFixture;

namespace Zomp.EFCore.WindowFunctions.Npgsql.Tests;

[Collection(nameof(NpgsqlCollection))]
public class CountTests : TestBase
{
    private readonly Testing.CountTests countTests;

    public CountTests(ITestOutputHelper output)
        : base(output)
    {
        countTests = new Testing.CountTests(DbContext);
    }

    [Fact]
    public void CountBasic()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<int, long>(r.Id, EF.Functions.Over()),
        });

        var result = query.ToList();

        var maxId = (long)TestFixture.TestRows.Length;
        var expectedSequence = Enumerable.Range(0, TestFixture.TestRows.Length).Select(_ => maxId);
        Assert.Equal(expectedSequence, result.Select(r => r.Count));
    }

    [Fact]
    public void CountBasicNullable()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<int?, long>(r.Col1, EF.Functions.Over()),
        });

        var result = query.ToList();

        var countId = TestFixture.TestRows.Count(x => x.Col1 is not null);
        var expectedSequence = Enumerable.Range(0, TestFixture.TestRows.Length).Select(_ => (long)countId);
        Assert.Equal(expectedSequence, result.Select(r => r.Count));
    }

    [Fact]
    public void CountBetweenCurrentRowAndNext()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<int, long>(r.Id, EF.Functions.Over().OrderBy(r.Id).Rows().FromCurrentRow().ToFollowing(1)),
        });

        var result = query.ToList();

        var expectedSequence = TestRows
            .Select((_, i) => i < TestRows.Length - 1 ? 2L : 1L);
        Assert.Equal(expectedSequence, result.Select(r => r.Count));
    }

    [Fact]
    public void CountBetweenCurrentRowAndNextNullable()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<int?, long>(r.Col1, EF.Functions.Over().OrderBy(r.Id).Rows().FromCurrentRow().ToFollowing(1)),
        });

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i)
            => (long)TestRows.CountNonNulls(z => z.Col1, i, i + 1));
        Assert.Equal(expectedSequence, result.Select(r => r.Count));
    }

    [Fact]
    public void CountBetweenTwoPreceding()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<int?, long>(r.Col1, EF.Functions.Over().OrderBy(r.Id).Rows().FromPreceding(2).ToPreceding(1)),
        });

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i)
            => (long)TestRows.CountNonNulls(z => z.Col1, i - 2, i - 1));
        Assert.Equal(expectedSequence, result.Select(r => r.Count));
    }

    [Fact]
    public void CountBetweenTwoFollowing()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<int?, long>(r.Col1, EF.Functions.Over().OrderBy(r.Id).Rows().FromFollowing(1).ToFollowing(2)),
        });

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i)
            => (long)TestRows.CountNonNulls(z => z.Col1, i + 1, i + 2));
        Assert.Equal(expectedSequence, result.Select(r => r.Count));
    }

    [Fact]
    public void CountBetweenFollowingAndUnbounded()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Original = r,
            Count = EF.Functions.Count<int?, long>(r.Col1, EF.Functions.Over().OrderBy(r.Id).Rows().FromFollowing(1).ToUnbounded()),
        })
        .OrderBy(r => r.Original.Id);

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i)
            => (long)TestRows.CountNonNulls(z => z.Col1, i + 1, TestRows.Length - 1));
        Assert.Equal(expectedSequence, result.Select(r => r.Count));
    }

    [Fact]
    public void CountWithPartition()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<int?, long>(
                r.Col1,
                EF.Functions.Over().PartitionBy(r.Id / 10)),
        });

        var result = query.ToList();

        var groups = TestRows.GroupBy(r => r.Id / 10)
            .ToDictionary(
            r => r.Key,
            r => (long)r.Count(z => z.Col1 is not null));

        var expectedSequence = TestRows.Select(r => groups[r.Id / 10]);
        Assert.Equal(expectedSequence, result.Select(r => r.Count));
    }

    [Fact]
    public void CountWith2Partitions()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Original = r,
            Count = EF.Functions.Count<int, long>(
                r.Id,
                EF.Functions.Over().PartitionBy(r.Id / 10).ThenBy(r.Date.DayOfYear % 2)),
        })
        .OrderBy(r => r.Original.Id);

        var result = query.ToList();

        var groups = TestRows.GroupBy(z => (z.Id / 10, z.Date.DayOfYear % 2))
            .ToDictionary(
            r => r.Key,
            r => (long)r.Count());

        var expectedSequence = TestRows.Select(r => groups[(r.Id / 10, r.Date.DayOfYear % 2)]);
        Assert.Equal(expectedSequence, result.Select(r => r.Count));
    }

    [Fact]
    public void SimpleCountWithCast()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<long, long>((long)r.Id, EF.Functions.Over()),
        });

        var result = query.ToList();

        var count = TestRows.Length;
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => (long)count);
        Assert.Equal(expectedSequence, result.Select(r => (long)r.Count));
    }

    [Fact]
    public void CountWithCastToString()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<string, long>(r.Col1.ToString(), EF.Functions.Over()),
        });

        var result = query.ToList();

        var count = TestRows.Count(r => r.Col1?.ToString(CultureInfo.InvariantCulture) != null);
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => (long)count);
        Assert.Equal(expectedSequence, result.Select(r => r.Count));
    }

    [Fact]
    public void CountBinary()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Count = EF.Functions.Count<byte[]?, long>(r.IdBytes, EF.Functions.Over()),
        });

        var result = query.ToList();

        var count = TestRows.Length;
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => (long)count);

        Assert.Equal(expectedSequence, result.Select(r => r.Count));
    }
}