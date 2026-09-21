namespace Zomp.EFCore.WindowFunctions.Testing;

/// <summary>
/// Select with an element index, translated to ROW_NUMBER, https://github.com/dotnet/efcore/issues/24218.
/// </summary>
/// <remarks>
/// Each query is run as is against <see cref="TestRows"/> for the expected result, so the index means what LINQ says it does.
/// </remarks>
public partial class SelectWithIndexTests
{
    [Test]
    public async Task AfterOrderBy()
    {
        var result = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select((r, i) => new { r.Id, Index = i })
            .ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select((r, i) => new { r.Id, Index = i });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task AfterOrderByDescendingThenBy()
    {
        var result = DbContext.TestRows
            .OrderByDescending(r => r.Id / 10)
            .ThenBy(r => r.Id)
            .Select((r, i) => new { r.Id, Position = i + 1 })
            .ToList();

        var expected = TestRows
            .OrderByDescending(r => r.Id / 10)
            .ThenBy(r => r.Id)
            .Select((r, i) => new { r.Id, Position = i + 1 });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task FilteredBeforeNumbering()
    {
        var result = DbContext.TestRows
            .Where(r => r.Id > 2)
            .OrderBy(r => r.Id)
            .Select((r, i) => new { r.Id, Index = i })
            .ToList();

        var expected = TestRows
            .Where(r => r.Id > 2)
            .OrderBy(r => r.Id)
            .Select((r, i) => new { r.Id, Index = i });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task FilteredAfterNumbering()
    {
        // EF Core would apply the filter first, which is why it does not translate the index (see the issue).
        var result = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select((r, i) => new { r.Id, Index = i })
            .Where(w => w.Id > 2)
            .ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select((r, i) => new { r.Id, Index = i })
            .Where(w => w.Id > 2);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task FilteredOnIndex()
    {
        var result = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select((r, i) => new { r.Id, Index = i })
            .Where(w => w.Index % 2 == 1)
            .ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select((r, i) => new { r.Id, Index = i })
            .Where(w => w.Index % 2 == 1);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task AfterSkip()
    {
        var result = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Skip(2)
            .Select((r, i) => new { r.Id, Index = i })
            .ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Skip(2)
            .Select((r, i) => new { r.Id, Index = i });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task WithoutOrderBy()
    {
        // Without an ordering the rows come in no defined order, so only the set of indexes is certain.
        var result = DbContext.TestRows
            .Select((r, i) => i)
            .ToList();

        await Assert.That(result).IsEquivalentTo(Enumerable.Range(0, TestRows.Length));
    }

    [Test]
    public async Task AfterProjection()
    {
        var result = DbContext.TestRows
            .OrderByDescending(r => r.Id)
            .Select(r => new { r.Id, r.Col1 })
            .Select((x, i) => new { x.Id, Index = i })
            .ToList();

        var expected = TestRows
            .OrderByDescending(r => r.Id)
            .Select(r => new { r.Id, r.Col1 })
            .Select((x, i) => new { x.Id, Index = i });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task AfterProjectionFilterAndSkip()
    {
        var result = DbContext.TestRows
            .OrderByDescending(r => r.Id)
            .Select(r => new { r.Id, r.Col1 })
            .Where(x => x.Id > 2)
            .Skip(1)
            .Select((x, i) => new { x.Id, Index = i })
            .ToList();

        var expected = TestRows
            .OrderByDescending(r => r.Id)
            .Select(r => new { r.Id, r.Col1 })
            .Where(x => x.Id > 2)
            .Skip(1)
            .Select((x, i) => new { x.Id, Index = i });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task NotTranslatedWhenTheOrderingCannotBeFollowed()
    {
        // The ordering is behind a projection with a window function, which cannot be folded into the Select without
        // changing the window. Numbering in some other order would be wrong, so the query is left for EF Core to reject.
        await Assert.That(() => DbContext.TestRows
                .OrderByDescending(r => r.Id)
                .Select(r => new { r.Id, RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(r.Id)) })
                .Select((x, i) => new { x.Id, Index = i })
                .ToList())
            .Throws<InvalidOperationException>()
            .WithMessageContaining("could not be translated", StringComparison.Ordinal);
    }
}
