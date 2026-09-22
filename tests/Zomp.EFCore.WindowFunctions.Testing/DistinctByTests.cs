namespace Zomp.EFCore.WindowFunctions.Testing;

/// <summary>
/// DistinctBy, translated to the first row of each key by ROW_NUMBER.
/// </summary>
/// <remarks>
/// Each query is run as is against <see cref="TestRows"/> for the expected result: the first row of each key, in the order of the source.
/// </remarks>
public partial class DistinctByTests
{
    [Test]
    public async Task FirstOfEachKey()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .DistinctBy(r => r.Id / 10);

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .DistinctBy(r => r.Id / 10);

        await Assert.That(result).IsEquivalentTo(expected, TestRowEqualityComparer.Default, CollectionOrdering.Matching);

        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task FirstOfEachKeyDescending()
    {
        var query = DbContext.TestRows
            .OrderByDescending(r => r.Id)
            .DistinctBy(r => r.Id / 10)
            .Select(r => r.Id);

        var result = query.ToList();

        var expected = TestRows
            .OrderByDescending(r => r.Id)
            .DistinctBy(r => r.Id / 10)
            .Select(r => r.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);

        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task CompositeKey()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .DistinctBy(r => new { Tens = r.Id / 10, Odd = r.Id % 2 })
            .Select(r => r.Id);

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .DistinctBy(r => new { Tens = r.Id / 10, Odd = r.Id % 2 })
            .Select(r => r.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);

        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task NullableKey()
    {
        // LINQ keeps one row for the null key, and PARTITION BY puts the nulls together.
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .DistinctBy(r => r.Col1 == null ? null : r.Col1 / 100)
            .Select(r => r.Id);

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .DistinctBy(r => r.Col1 == null ? null : r.Col1 / 100)
            .Select(r => r.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);

        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task FilteredBeforeAndAfter()
    {
        var query = DbContext.TestRows
            .Where(r => r.Id > 2)
            .OrderBy(r => r.Id)
            .DistinctBy(r => r.Id / 10)
            .Where(r => r.Id > 5)
            .Select(r => r.Id);

        var result = query.ToList();

        var expected = TestRows
            .Where(r => r.Id > 2)
            .OrderBy(r => r.Id)
            .DistinctBy(r => r.Id / 10)
            .Where(r => r.Id > 5)
            .Select(r => r.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);

        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task TakeAfterDistinctBy()
    {
        var query = DbContext.TestRows
            .OrderByDescending(r => r.Id)
            .DistinctBy(r => r.Id / 10)
            .Take(2)
            .Select(r => r.Id);

        var result = query.ToList();

        var expected = TestRows
            .OrderByDescending(r => r.Id)
            .DistinctBy(r => r.Id / 10)
            .Take(2)
            .Select(r => r.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);

        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task WithoutOrderBy()
    {
        // Without an ordering any row of each key may come first, so only the keys are certain.
        var query = DbContext.TestRows
            .DistinctBy(r => r.Id / 10)
            .Select(r => r.Id / 10);

        var result = query.ToList();

        await Assert.That(result).IsEquivalentTo(TestRows.Select(r => r.Id / 10).Distinct());

        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task CountOfKeys()
    {
        var query = DbContext.TestRows
            .DistinctBy(r => r.Id / 10);

        var result = query.Count();

        await Assert.That(result).IsEqualTo(TestRows.Select(r => r.Id / 10).Distinct().Count());

        await Verify(query.ToQueryString());
    }
}
