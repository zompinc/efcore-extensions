namespace Zomp.EFCore.WindowFunctions.Testing;

/// <summary>
/// Where with an element index, translated to ROW_NUMBER.
/// </summary>
/// <remarks>
/// Each query is run as is against <see cref="TestRows"/> for the expected result, so the index means what LINQ says it does.
/// </remarks>
public partial class WhereWithIndexTests
{
    [Test]
    public async Task EveryOtherRow()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Where((r, i) => i % 2 == 0);

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Where((r, i) => i % 2 == 0);

        await Assert.That(result).IsEquivalentTo(expected, TestRowEqualityComparer.Default, CollectionOrdering.Matching);

        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task FilteredBeforeNumbering()
    {
        var query = DbContext.TestRows
            .Where(r => r.Id > 2)
            .OrderBy(r => r.Id)
            .Where((r, i) => i < 3)
            .Select(r => r.Id);

        var result = query.ToList();

        var expected = TestRows
            .Where(r => r.Id > 2)
            .OrderBy(r => r.Id)
            .Where((r, i) => i < 3)
            .Select(r => r.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);

        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task FilteredAfterNumbering()
    {
        var query = DbContext.TestRows
            .OrderByDescending(r => r.Id)
            .Where((r, i) => i > 0)
            .Where(r => r.Id > 5)
            .Select(r => r.Id);

        var result = query.ToList();

        var expected = TestRows
            .OrderByDescending(r => r.Id)
            .Where((r, i) => i > 0)
            .Where(r => r.Id > 5)
            .Select(r => r.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);

        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task TakeAfterFilter()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Where((r, i) => i % 2 == 1)
            .Take(2)
            .Select(r => r.Id);

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Where((r, i) => i % 2 == 1)
            .Take(2)
            .Select(r => r.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);

        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task ElementAndIndex()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Where((r, i) => r.Id > i * 3)
            .Select(r => r.Id);

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Where((r, i) => r.Id > i * 3)
            .Select(r => r.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);

        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task AfterProjection()
    {
        var query = DbContext.TestRows
            .OrderByDescending(r => r.Id)
            .Select(r => new { r.Id, r.Col1 })
            .Where((x, i) => i % 3 == 0);

        var result = query.ToList();

        var expected = TestRows
            .OrderByDescending(r => r.Id)
            .Select(r => new { r.Id, r.Col1 })
            .Where((x, i) => i % 3 == 0);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);

        await Verify(query.ToQueryString());
    }
}
