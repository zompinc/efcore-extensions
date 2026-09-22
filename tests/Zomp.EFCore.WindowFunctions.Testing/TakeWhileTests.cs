namespace Zomp.EFCore.WindowFunctions.Testing;

/// <summary>
/// TakeWhile and SkipWhile, translated to a running count of the rows that fail the predicate.
/// </summary>
/// <remarks>
/// Each query is run as is against <see cref="TestRows"/> for the expected result. Unlike Where, both stop at the first row
/// that fails the predicate, whatever the rows after it.
/// </remarks>
public partial class TakeWhileTests
{
    [Test]
    public async Task TakeWhileBasic()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .TakeWhile(r => r.Id < 10)
            .Select(r => r.Id);

        var result = query.ToList();

        var expected = TestRows.OrderBy(r => r.Id).TakeWhile(r => r.Id < 10).Select(r => r.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task TakeWhileStopsAtTheFirstFailure()
    {
        // The rows after 7 pass again, which Where would keep.
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .TakeWhile(r => r.Id != 7);

        var result = query.ToList();

        var expected = TestRows.OrderBy(r => r.Id).TakeWhile(r => r.Id != 7);

        await Assert.That(result).IsEquivalentTo(expected, TestRowEqualityComparer.Default, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task SkipWhileBasic()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .SkipWhile(r => r.Id != 7)
            .Select(r => r.Id);

        var result = query.ToList();

        var expected = TestRows.OrderBy(r => r.Id).SkipWhile(r => r.Id != 7).Select(r => r.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task ComparisonWithNullIsFalse()
    {
        // The first row has no Col1, and null < 100 is false in C#, so nothing is taken and nothing skipped.
        var take = DbContext.TestRows.OrderBy(r => r.Id).TakeWhile(r => r.Col1 < 100).Select(r => r.Id);
        var query = DbContext.TestRows.OrderBy(r => r.Id).SkipWhile(r => r.Col1 < 100).Select(r => r.Id);

        var taken = take.ToList();
        var result = query.ToList();

        await Assert.That(taken).IsEquivalentTo(TestRows.OrderBy(r => r.Id).TakeWhile(r => r.Col1 < 100).Select(r => r.Id));
        await Assert.That(result).IsEquivalentTo(TestRows.OrderBy(r => r.Id).SkipWhile(r => r.Col1 < 100).Select(r => r.Id), CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    [Skip("EF Core translates a negated nullable comparison in the condition of a ternary as NOT (...), which is NULL rather than true when the column is NULL: https://github.com/dotnet/efcore/issues/39059")]
    public async Task SkipWhileWithNegatedNullableComparison()
    {
        // !(null <= 0) is true in C#, so the first two rows are skipped: Col1 is null, then 10.
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .SkipWhile(r => !(r.Col1 <= 0))
            .Select(r => r.Id);

        var result = query.ToList();

        var expected = TestRows.OrderBy(r => r.Id).SkipWhile(r => !(r.Col1 <= 0)).Select(r => r.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task TakeWhileDescendingThenBy()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id / 10)
            .ThenByDescending(r => r.Id)
            .TakeWhile(r => r.Id != 3)
            .Select(r => r.Id);

        var result = query.ToList();

        var expected = TestRows.OrderBy(r => r.Id / 10).ThenByDescending(r => r.Id).TakeWhile(r => r.Id != 3).Select(r => r.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task TakeWhileAfterProjection()
    {
        var query = DbContext.TestRows
            .OrderByDescending(r => r.Id)
            .Select(r => new { r.Id, r.Col1 })
            .TakeWhile(x => x.Id > 12);

        var result = query.ToList();

        var expected = TestRows.OrderByDescending(r => r.Id).Select(r => new { r.Id, r.Col1 }).TakeWhile(x => x.Id > 12);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task FilteredBeforeAndTakeAfter()
    {
        var query = DbContext.TestRows
            .Where(r => r.Id > 2)
            .OrderBy(r => r.Id)
            .SkipWhile(r => r.Id < 7)
            .Take(2)
            .Select(r => r.Id);

        var result = query.ToList();

        var expected = TestRows.Where(r => r.Id > 2).OrderBy(r => r.Id).SkipWhile(r => r.Id < 7).Take(2).Select(r => r.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task NotTranslatedWithoutAnOrdering()
    {
        // Without an ordering the rows come in no defined order, so there is no first row that fails.
        var query = DbContext.TestRows.TakeWhile(r => r.Id < 10);

        await Assert.That(() => query.ToList())
            .Throws<InvalidOperationException>()
            .WithMessageContaining("could not be translated", StringComparison.Ordinal);
    }
}
