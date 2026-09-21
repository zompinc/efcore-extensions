namespace Zomp.EFCore.WindowFunctions.Testing;

public partial class MaxTests
{
    [Test]
    public async Task SimpleMax()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Max(r.Id, EF.Functions.Over()));

        var result = query.ToList();

        var maxId = TestRows.Max(r => r.Id);
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => (int?)maxId);
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MaxDifferByExpressionOnly()
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
        await Assert.That(distinctResults.Max).IsEqualTo(expectedMax);
        await Assert.That(distinctResults.MaxTimesTwo).IsEqualTo(expectedMaxTimesTwo);
    }

    [Test]
    public void MaxWithOrder()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Max(r.Id, EF.Functions.Over().OrderBy(r.Col1 / 10)));

        var result = query.ToList();
    }

    [Test]
    public async Task SimpleMaxNullable()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Max(r.Col1, EF.Functions.Over()));

        var result = query.ToList();

        var maxId = TestRows.Max(r => r.Col1);
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => maxId);
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MaxBetweenCurrentRowAndOne()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Max(r.Id, EF.Functions.Over().OrderBy(r.Id).Rows().FromCurrentRow().ToFollowing(1)));

        var result = query.ToList();

        var maxId = TestRows.Max(r => r.Id);
        var expectedSequence = TestRows
            .Select((_, i)
            => (int?)(i < TestRows.Length - 1
            ? Math.Max(TestRows[i].Id, TestRows[i + 1].Id)
            : TestRows[i].Id));
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MaxBetweenTwoPreceding()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Max(r.Id, EF.Functions.Over().OrderBy(r.Id).Rows().FromPreceding(2).ToPreceding(1)));

        var result = query.ToList();

        var expectedSequence = TestRows
            .Select((_, i)
            => i == 0 ? (int?)null
            : i == 1 ? TestRows[0].Id
            : Math.Max(TestRows[i - 2].Id, TestRows[i - 1].Id));
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MaxBetweenTwoFollowing()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Max(r.Id, EF.Functions.Over().OrderBy(r.Id).Rows().FromFollowing(1).ToFollowing(2)));

        var result = query.ToList();

        var expectedSequence = TestRows
            .Select((_, i)
            => i < TestRows.Length - 2
            ? Math.Max(TestRows[i + 1].Id, TestRows[i + 2].Id)
            : i < TestRows.Length - 1 ? TestRows[i + 1].Id : (int?)null);
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MaxBetweenFollowingAndUnbounded()
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
        await Assert.That(result.Select(r => r.Max)).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MaxWithPartition()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Max(
                r.Id,
                EF.Functions.Over().PartitionBy(r.Id / 10)));

        var result = query.ToList();

        var groups = TestRows.GroupBy(r => r.Id / 10)
            .ToDictionary(r => r.Key, r => r.Max(s => s.Id));

        var expectedSequence = TestRows.Select(r => (int?)groups[r.Id / 10]);
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MaxWith2Partitions()
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
        await Assert.That(result.Select(r => r.Max)).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task SimpleMaxWithCast()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Max((long)r.Id, EF.Functions.Over()));

        var result = query.ToList();

        var maxId = TestRows.Max(r => r.Id);
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => (long?)maxId);
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MaxWithCastToString()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Max(r.Col1.ToString(), EF.Functions.Over()));

        var result = query.ToList();

        var max = TestRows.Max(r => r.Col1?.ToString(CultureInfo.InvariantCulture));
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => max);
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MaxBinary()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Max(r.IdBytes, EF.Functions.Over()));

        var result = query.ToList();

        var maxId = TestRows.Max(r => BitConverter.ToInt16(r.IdBytes));
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => (byte[]?)BitConverter.GetBytes(maxId));
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }
}