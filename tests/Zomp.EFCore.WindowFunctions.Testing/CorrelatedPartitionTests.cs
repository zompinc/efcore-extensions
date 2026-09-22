namespace Zomp.EFCore.WindowFunctions.Testing;

/// <summary>
/// Aggregates of a correlated subquery over the same rows, matched on a key of the current row, translated to window functions
/// partitioned by the key.
/// </summary>
/// <remarks>
/// Each query is run as is against <see cref="TestRows"/> for the expected result. The subquery must read the same rows as the
/// outer query, filters included, or it is left as a correlated subquery.
/// </remarks>
public partial class CorrelatedPartitionTests
{
    [Test]
    public async Task CountOfSameKey()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, InGroup = DbContext.TestRows.Count(t => t.Id / 10 == r.Id / 10) });

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, InGroup = TestRows.Count(t => t.Id / 10 == r.Id / 10) });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task WhereThenCountAndLongCount()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                InGroup = DbContext.TestRows.Where(t => r.Id / 10 == t.Id / 10).Count(),
                InGroupLong = DbContext.TestRows.Where(t => t.Id / 10 == r.Id / 10).LongCount(),
            });

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                InGroup = TestRows.Where(t => r.Id / 10 == t.Id / 10).Count(),
                InGroupLong = TestRows.Where(t => t.Id / 10 == r.Id / 10).LongCount(),
            });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task SumMinMaxAverageOfSameKey()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                Total = DbContext.TestRows.Where(t => t.Id / 10 == r.Id / 10).Sum(t => t.Id),
                Min = DbContext.TestRows.Where(t => t.Id / 10 == r.Id / 10).Min(t => t.Col1),
                Max = DbContext.TestRows.Where(t => t.Id / 10 == r.Id / 10).Max(t => t.Id),
                Average = DbContext.TestRows.Where(t => t.Id / 10 == r.Id / 10).Average(t => t.Id),
            });

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                Total = TestRows.Where(t => t.Id / 10 == r.Id / 10).Sum(t => t.Id),
                Min = TestRows.Where(t => t.Id / 10 == r.Id / 10).Min(t => t.Col1),
                Max = TestRows.Where(t => t.Id / 10 == r.Id / 10).Max(t => t.Id),
                Average = TestRows.Where(t => t.Id / 10 == r.Id / 10).Average(t => t.Id),
            });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task TwoKeys()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, InGroup = DbContext.TestRows.Count(t => t.Id / 10 == r.Id / 10 && r.Id % 2 == t.Id % 2) });

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, InGroup = TestRows.Count(t => t.Id / 10 == r.Id / 10 && r.Id % 2 == t.Id % 2) });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task NullableKeyAndSumOfNulls()
    {
        // null == null is true in C#, and PARTITION BY puts the nulls together; the rows without Col1 sum to 0.
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                SameCol1 = DbContext.TestRows.Count(t => t.Col1 == r.Col1),
                TotalCol1 = DbContext.TestRows.Where(t => t.Col1 == r.Col1).Sum(t => t.Col1),
            });

        var result = query.ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => new
            {
                r.Id,
                SameCol1 = TestRows.Count(t => t.Col1 == r.Col1),
                TotalCol1 = TestRows.Where(t => t.Col1 == r.Col1).Sum(t => t.Col1),
            });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task SameFilterInsideAndOut()
    {
        var query = DbContext.TestRows
            .Where(x => x.Id > 2)
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, InGroup = DbContext.TestRows.Where(x => x.Id > 2).Count(t => t.Id / 10 == r.Id / 10) });

        var result = query.ToList();

        var expected = TestRows
            .Where(x => x.Id > 2)
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, InGroup = TestRows.Where(x => x.Id > 2).Count(t => t.Id / 10 == r.Id / 10) });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task DifferentFilterIsLeftCorrelated()
    {
        // The subquery reads rows the outer query filters out, which a window over the outer rows cannot see.
        var query = DbContext.TestRows
            .Where(x => x.Id > 2)
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, InGroup = DbContext.TestRows.Count(t => t.Id / 10 == r.Id / 10) });

        var result = query.ToList();

        var expected = TestRows
            .Where(x => x.Id > 2)
            .OrderBy(r => r.Id)
            .Select(r => new { r.Id, InGroup = TestRows.Count(t => t.Id / 10 == r.Id / 10) });

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }

    [Test]
    public async Task FilterAfterTheWindow()
    {
        var query = DbContext.TestRows
            .Select(r => new { r.Id, InGroup = DbContext.TestRows.Count(t => t.Id / 10 == r.Id / 10) })
            .Where(x => x.InGroup > 1)
            .OrderBy(x => x.Id);

        var result = query.ToList();

        var expected = TestRows
            .Select(r => new { r.Id, InGroup = TestRows.Count(t => t.Id / 10 == r.Id / 10) })
            .Where(x => x.InGroup > 1)
            .OrderBy(x => x.Id);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Verify(query.ToQueryString());
    }
}
