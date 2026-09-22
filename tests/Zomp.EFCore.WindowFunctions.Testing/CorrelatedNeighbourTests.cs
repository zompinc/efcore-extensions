namespace Zomp.EFCore.WindowFunctions.Testing;

/// <summary>
/// A value of the previous or next row, read with a correlated subquery, translated to LAG and LEAD.
/// </summary>
/// <remarks>
/// Each query is run as is against <see cref="TestRows"/> for the expected result. The ordering must be unique, here the primary
/// key: LAG reads the row before the current one, which is the row with the greatest smaller key only when no two rows share it.
/// </remarks>
public partial class CorrelatedNeighbourTests
{
    [Test]
    public async Task PreviousAndNext()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                PreviousId = DbContext.TestRows.Where(t => t.Id < r.Id).OrderByDescending(t => t.Id).Select(t => (int?)t.Id).FirstOrDefault(),
                NextCol1 = DbContext.TestRows.Where(t => t.Id > r.Id).OrderBy(t => t.Id).Select(t => t.Col1).FirstOrDefault(),
            });

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                PreviousId = TestRows.Where(t => t.Id < r.Id).OrderByDescending(t => t.Id).Select(t => (int?)t.Id).FirstOrDefault(),
                NextCol1 = TestRows.Where(t => t.Id > r.Id).OrderBy(t => t.Id).Select(t => t.Col1).FirstOrDefault(),
            });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task NonNullableValueDefaultsForTheFirstRow()
    {
        // FirstOrDefault of an int is 0 when there is no previous row, where LAG is NULL.
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                PreviousId = DbContext.TestRows.Where(t => t.Id < r.Id).OrderByDescending(t => t.Id).Select(t => t.Id).FirstOrDefault(),
                PreviousDate = DbContext.TestRows.Where(t => r.Id > t.Id).OrderByDescending(t => t.Id).Select(t => t.Date).FirstOrDefault(),
            });

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                PreviousId = TestRows.Where(t => t.Id < r.Id).OrderByDescending(t => t.Id).Select(t => t.Id).FirstOrDefault(),
                PreviousDate = TestRows.Where(t => r.Id > t.Id).OrderByDescending(t => t.Id).Select(t => t.Date).FirstOrDefault(),
            });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task PreviousWithinKey()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                PreviousInTens = DbContext.TestRows
                    .Where(t => t.Id / 10 == r.Id / 10 && t.Id < r.Id)
                    .OrderByDescending(t => t.Id)
                    .Select(t => (int?)t.Id)
                    .FirstOrDefault(),
            });

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                PreviousInTens = TestRows
                    .Where(t => t.Id / 10 == r.Id / 10 && t.Id < r.Id)
                    .OrderByDescending(t => t.Id)
                    .Select(t => (int?)t.Id)
                    .FirstOrDefault(),
            });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task NonUniqueOrderingIsLeftCorrelated()
    {
        // Several rows share Id / 10, so the row before the current one need not be one with the greatest smaller key.
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                PreviousTens = DbContext.TestRows.Where(t => t.Id / 10 < r.Id / 10).OrderByDescending(t => t.Id / 10).Select(t => (int?)(t.Id / 10)).FirstOrDefault(),
            });

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                PreviousTens = TestRows.Where(t => t.Id / 10 < r.Id / 10).OrderByDescending(t => t.Id / 10).Select(t => (int?)(t.Id / 10)).FirstOrDefault(),
            });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task OrderingAgainstTheComparisonIsLeftCorrelated()
    {
        // Ordered ascending over the rows before the current one, this is the first row, not the previous one.
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, FirstId = DbContext.TestRows.Where(t => t.Id < r.Id).OrderBy(t => t.Id).Select(t => (int?)t.Id).FirstOrDefault() });

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, FirstId = TestRows.Where(t => t.Id < r.Id).OrderBy(t => t.Id).Select(t => (int?)t.Id).FirstOrDefault() });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }
}
