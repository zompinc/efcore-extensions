namespace Zomp.EFCore.WindowFunctions.Testing;

public partial class MaxTests
{
    [Fact]
    public void SimpleMax()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.Id, EF.Functions.Over()),
        });

        var result = query.ToList();

        var maxId = TestRows.Max(r => r.Id);
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => (int?)maxId);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    [Fact]
    public void MaxDifferByExpressionOnly()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.Id, EF.Functions.Over()),
            MaxTimesTwo = EF.Functions.Max(r.Id * 2, EF.Functions.Over()),
        });

        var result = query.ToList();

        var expectedMax = TestRows.Max(r => r.Id);
        var expectedMaxTimesTwo = TestRows.Max(r => r.Id * 2);

        var distinctResults = result.Distinct().Single();
        Assert.Equal(expectedMax, distinctResults.Max);
        Assert.Equal(expectedMaxTimesTwo, distinctResults.MaxTimesTwo);
    }

    [Fact]
    public void MaxWithOrder()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.Id, EF.Functions.Over().OrderBy(r.Col1 / 10)),
        });

        var result = query.ToList();
    }

    [Fact]
    public void SimpleMaxNullable()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.Col1, EF.Functions.Over()),
        });

        var result = query.ToList();

        var maxId = TestRows.Max(r => r.Col1);
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => maxId);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    [Fact]
    public void MaxBetweenCurrentRowAndOne()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.Id, EF.Functions.Over().OrderBy(r.Id).Rows().FromCurrentRow().ToFollowing(1)),
        });

        var result = query.ToList();

        var maxId = TestRows.Max(r => r.Id);
        var expectedSequence = TestRows
            .Select((_, i)
            => (int?)(i < TestRows.Length - 1
            ? Math.Max(TestRows[i].Id, TestRows[i + 1].Id)
            : TestRows[i].Id));
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    [Fact]
    public void MaxBetweenTwoPreceding()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.Id, EF.Functions.Over().OrderBy(r.Id).Rows().FromPreceding(2).ToPreceding(1)),
        });

        var result = query.ToList();

        var expectedSequence = TestRows
            .Select((_, i)
            => i == 0 ? (int?)null
            : i == 1 ? TestRows[0].Id
            : Math.Max(TestRows[i - 2].Id, TestRows[i - 1].Id));
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    [Fact]
    public void MaxBetweenTwoFollowing()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.Id, EF.Functions.Over().OrderBy(r.Id).Rows().FromFollowing(1).ToFollowing(2)),
        });

        var result = query.ToList();

        var expectedSequence = TestRows
            .Select((_, i)
            => i < TestRows.Length - 2
            ? Math.Max(TestRows[i + 1].Id, TestRows[i + 2].Id)
            : i < TestRows.Length - 1 ? TestRows[i + 1].Id : (int?)null);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    [Fact]
    public void MaxBetweenFollowingAndUnbounded()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Original = r,
            Max = EF.Functions.Max(r.Id, EF.Functions.Over().OrderBy(r.Id).Rows().FromFollowing(1).ToUnbounded()),
        })
        .OrderBy(r => r.Original.Id);

        var result = query.ToList();

        var maxId = TestRows.Max(r => r.Id);
        var expectedSequence = TestRows
            .Select((_, i)
            => i < TestRows.Length - 1 ? maxId : (int?)null);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    [Fact]
    public void MaxWithPartition()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(
                r.Id,
                EF.Functions.Over().PartitionBy(r.Id / 10)),
        });

        var result = query.ToList();

        var groups = TestRows.GroupBy(r => r.Id / 10)
            .ToDictionary(r => r.Key, r => r.Max(s => s.Id));

        var expectedSequence = TestRows.Select(r => (int?)groups[r.Id / 10]);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    [Fact]
    public void MaxWith2Partitions()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Original = r,
            Max = EF.Functions.Max(
                r.Id,
                EF.Functions.Over().PartitionBy(r.Id / 10).ThenBy(r.Date.DayOfYear % 2)),
        })
        .OrderBy(r => r.Original.Id);

        var result = query.ToList();

        var groups = TestRows.GroupBy(z => (z.Id / 10, z.Date.DayOfYear % 2))
            .ToDictionary(r => r.Key, r => r.Max(s => (int?)s.Id));

        var expectedSequence = TestRows.Select(r => groups[(r.Id / 10, r.Date.DayOfYear % 2)]);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    [Fact]
    public void SimpleMaxWithCast()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max((long)r.Id, EF.Functions.Over()),
        });

        var result = query.ToList();

        var maxId = TestRows.Max(r => r.Id);
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => (long?)maxId);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    [Fact]
    public void MaxWithCastToString()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.Col1.ToString(), EF.Functions.Over()),
        });

        var result = query.ToList();

        var max = TestRows.Max(r => r.Col1?.ToString(CultureInfo.InvariantCulture));
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => max);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    [SkippableFact]
    public void MaxBinary()
    {
        Skip.If(DbContext.IsPostgreSQL, "Can't max over bit(n) or bytea in postgres");

        var query = DbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.IdBytes, EF.Functions.Over()),
        });

        var result = query.ToList();

        var maxId = TestRows.Max(r => BitConverter.ToInt16(r.IdBytes));
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => BitConverter.GetBytes(maxId));
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }
}