namespace Zomp.EFCore.WindowFunctions.Testing;

/// <summary>
/// GroupBy followed by SelectMany over each group, translated to window functions partitioned by the key.
/// </summary>
/// <remarks>
/// Each query is run as is against <see cref="TestRows"/> for the expected result. The rows are put in order afterwards,
/// since SQL returns them in no particular order.
/// </remarks>
public partial class GroupWindowTests
{
    [Test]
    public async Task IndexWithinGroup()
    {
        var query = DbContext.TestRows
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => g.OrderBy(r => r.Id).Select((r, i) => new { r.Id, Index = i }))
            .OrderBy(x => x.Id);

        var result = query.ToList();

        var expected = TestRows
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => g.OrderBy(r => r.Id).Select((r, i) => new { r.Id, Index = i }))
            .OrderBy(x => x.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task IndexWithinGroupDescending()
    {
        var query = DbContext.TestRows
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => g.OrderByDescending(r => r.Id).Select((r, i) => new { r.Id, Position = i + 1 }))
            .OrderBy(x => x.Id);

        var result = query.ToList();

        var expected = TestRows
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => g.OrderByDescending(r => r.Id).Select((r, i) => new { r.Id, Position = i + 1 }))
            .OrderBy(x => x.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task IndexFollowsTheSourceOrder()
    {
        // GroupBy keeps the order of the rows it is given within each group.
        var query = DbContext.TestRows
            .OrderByDescending(r => r.Id)
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => g.Select((r, i) => new { r.Id, Index = i }))
            .OrderBy(x => x.Id);

        var result = query.ToList();

        var expected = TestRows
            .OrderByDescending(r => r.Id)
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => g.Select((r, i) => new { r.Id, Index = i }))
            .OrderBy(x => x.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task CountOfGroup()
    {
        var query = DbContext.TestRows
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => g.Select(r => new { r.Id, InGroup = g.Count(), InGroupLong = g.LongCount() }))
            .OrderBy(x => x.Id);

        var result = query.ToList();

        var expected = TestRows
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => g.Select(r => new { r.Id, InGroup = g.Count(), InGroupLong = g.LongCount() }))
            .OrderBy(x => x.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task SumMinMaxOfGroup()
    {
        var query = DbContext.TestRows
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => g.Select(r => new
            {
                r.Id,
                Total = g.Sum(t => t.Id),
                TotalCol1 = g.Sum(t => t.Col1),
                Min = g.Min(t => t.Col1),
                Max = g.Max(t => t.Id),
            }))
            .OrderBy(x => x.Id);

        var result = query.ToList();

        var expected = TestRows
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => g.Select(r => new
            {
                r.Id,
                Total = g.Sum(t => t.Id),
                TotalCol1 = g.Sum(t => t.Col1),
                Min = g.Min(t => t.Col1),
                Max = g.Max(t => t.Id),
            }))
            .OrderBy(x => x.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task SumOfNullsIsZero()
    {
        // LINQ sums a group of nulls to 0, where SQL's SUM returns NULL.
        var query = DbContext.TestRows
            .GroupBy(r => r.Col1 == null)
            .SelectMany(g => g.Select(r => new { r.Id, Total = g.Sum(t => t.Col1) }))
            .OrderBy(x => x.Id);

        var result = query.ToList();

        var expected = TestRows
            .GroupBy(r => r.Col1 == null)
            .SelectMany(g => g.Select(r => new { r.Id, Total = g.Sum(t => t.Col1) }))
            .OrderBy(x => x.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task AverageOfGroup()
    {
        var query = DbContext.TestRows
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => g.Select(r => new { r.Id, Average = g.Average(t => t.Id) }))
            .OrderBy(x => x.Id);

        var result = query.ToList();

        var expected = TestRows
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => g.Select(r => new { r.Id, Average = g.Average(t => t.Id) }))
            .OrderBy(x => x.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task KeyAndCompositeKey()
    {
        var query = DbContext.TestRows
            .GroupBy(r => new { Tens = r.Id / 10, Odd = r.Id % 2 })
            .SelectMany(g => g.Select(r => new { r.Id, g.Key.Tens, g.Key.Odd, InGroup = g.Count() }))
            .OrderBy(x => x.Id);

        var result = query.ToList();

        var expected = TestRows
            .GroupBy(r => new { Tens = r.Id / 10, Odd = r.Id % 2 })
            .SelectMany(g => g.Select(r => new { r.Id, g.Key.Tens, g.Key.Odd, InGroup = g.Count() }))
            .OrderBy(x => x.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task IndexAndCountTogether()
    {
        var query = DbContext.TestRows
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => g.OrderBy(r => r.Id).Select((r, i) => new { r.Id, Position = i + 1, Of = g.Count() }))
            .OrderBy(x => x.Id);

        var result = query.ToList();

        var expected = TestRows
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => g.OrderBy(r => r.Id).Select((r, i) => new { r.Id, Position = i + 1, Of = g.Count() }))
            .OrderBy(x => x.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task FilteredBeforeAndAfter()
    {
        var query = DbContext.TestRows
            .Where(r => r.Id > 2)
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => g.OrderBy(r => r.Id).Select((r, i) => new { r.Id, Index = i, InGroup = g.Count() }))
            .Where(x => x.InGroup > 1)
            .OrderBy(x => x.Id);

        var result = query.ToList();

        var expected = TestRows
            .Where(r => r.Id > 2)
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => g.OrderBy(r => r.Id).Select((r, i) => new { r.Id, Index = i, InGroup = g.Count() }))
            .Where(x => x.InGroup > 1)
            .OrderBy(x => x.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task NotTranslatedForADistinctCount()
    {
        // COUNT(DISTINCT ...) is not allowed over a window on SQL Server or PostgreSQL, so the query is left for EF Core
        // to reject.
        var query = DbContext.TestRows
            .GroupBy(r => r.Id / 10)
            .SelectMany(g => g.Select(r => new { r.Id, Distinct = g.Select(t => t.Col1).Distinct().Count() }));

        await Assert.That(() => query.ToList())
            .Throws<InvalidOperationException>()
            .WithMessageContaining("could not be translated", StringComparison.Ordinal);
    }
}
