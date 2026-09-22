namespace Zomp.EFCore.WindowFunctions.Testing;

/// <summary>
/// Aggregates of a correlated subquery over the rows before the current one in some order, translated to RANK and to aggregates
/// over an ordered window.
/// </summary>
/// <remarks>
/// Each query is run as is against <see cref="TestRows"/> for the expected result. The ordering column must not be nullable:
/// in C# a comparison with null is false, which an ordered window cannot express.
/// </remarks>
public partial class CorrelatedRunningTests
{
    [Test]
    public async Task CountBeforeIsRank()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                Position = DbContext.TestRows.Count(t => t.Id < r.Id),
                GroupRank = DbContext.TestRows.Count(t => t.Id / 10 < r.Id / 10) + 1,
            });

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                Position = TestRows.Count(t => t.Id < r.Id),
                GroupRank = TestRows.Count(t => t.Id / 10 < r.Id / 10) + 1,
            });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task CountAfterIsRankDescending()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, Above = DbContext.TestRows.LongCount(t => t.Id / 10 > r.Id / 10) });

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, Above = TestRows.LongCount(t => t.Id / 10 > r.Id / 10) });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task CountUpToIncludesTies()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                UpTo = DbContext.TestRows.Count(t => t.Id / 10 <= r.Id / 10),
                From = DbContext.TestRows.Count(t => r.Id / 10 <= t.Id / 10),
            });

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                UpTo = TestRows.Count(t => t.Id / 10 <= r.Id / 10),
                From = TestRows.Count(t => r.Id / 10 <= t.Id / 10),
            });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task RunningSumMaxAndAverage()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                RunningTotal = DbContext.TestRows.Where(t => t.Id <= r.Id).Sum(t => t.Id),
                RunningMax = DbContext.TestRows.Where(t => t.Id <= r.Id).Max(t => t.Col1),
                RunningAverage = DbContext.TestRows.Where(t => t.Id <= r.Id).Average(t => t.Id),
            });

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                RunningTotal = TestRows.Where(t => t.Id <= r.Id).Sum(t => t.Id),
                RunningMax = TestRows.Where(t => t.Id <= r.Id).Max(t => t.Col1),
                RunningAverage = TestRows.Where(t => t.Id <= r.Id).Average(t => t.Id),
            });

        await Assert.That(result.Select(x => (x.Id, x.RunningTotal, x.RunningMax, Math.Round(x.RunningAverage, 8))))
            .IsEquivalentTo(expected.Select(x => (x.Id, x.RunningTotal, x.RunningMax, Math.Round(x.RunningAverage, 8))), CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task RunningSumWithinKey()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, RunningTotal = DbContext.TestRows.Where(t => t.Id / 10 == r.Id / 10 && r.Id >= t.Id).Sum(t => t.Id) });

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, RunningTotal = TestRows.Where(t => t.Id / 10 == r.Id / 10 && r.Id >= t.Id).Sum(t => t.Id) });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task NullableOrderingIsLeftCorrelated()
    {
        // null < x is false in C#, so the rows without Col1 count 0, which RANK over Col1 does not give.
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, Below = DbContext.TestRows.Count(t => t.Col1 < r.Col1) });

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, Below = TestRows.Count(t => t.Col1 < r.Col1) });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task StrictRunningSumIsLeftCorrelated()
    {
        // With ties, the rows strictly before are not a window frame of the ordered rows.
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, Before = DbContext.TestRows.Where(t => t.Id / 10 < r.Id / 10).Sum(t => t.Id) });

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, Before = TestRows.Where(t => t.Id / 10 < r.Id / 10).Sum(t => t.Id) });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }
}
