namespace Zomp.EFCore.WindowFunctions.Testing;

/// <summary>
/// Verifies that enabling window functions leaves provider specific paging SQL intact.
/// </summary>
/// <remarks>
/// Regression tests for https://github.com/zompinc/efcore-extensions/issues/23, where the SQLite
/// provider fell back to the generic query SQL generator and emitted ANSI
/// <c>OFFSET ... FETCH NEXT</c> instead of SQLite's <c>LIMIT</c>.
/// </remarks>
public partial class PagingTests
{
    [Fact]
    public void First()
    {
        var result = DbContext.TestRows.OrderBy(r => r.Id).First();

        var expected = TestRows.OrderBy(r => r.Id).First();

        Assert.Equal(expected, result, TestRowEqualityComparer.Default);
    }

    [Fact]
    public void Take()
    {
        var result = DbContext.TestRows.OrderBy(r => r.Id).Take(3).ToList();

        var expected = TestRows.OrderBy(r => r.Id).Take(3);

        Assert.Equal(expected, result, TestRowEqualityComparer.Default);
    }

    [Fact]
    public void SkipAndTake()
    {
        var result = DbContext.TestRows.OrderBy(r => r.Id).Skip(2).Take(3).ToList();

        var expected = TestRows.OrderBy(r => r.Id).Skip(2).Take(3);

        Assert.Equal(expected, result, TestRowEqualityComparer.Default);
    }

    [Fact]
    public void TakeWithWindowFunction()
    {
        var query = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(r.Id)))
            .Take(3);

        var result = query.ToList();

        Assert.Equal([1L, 2L, 3L], result);
    }

    [Fact]
    public void TakeBetweenWindowFunctions()
    {
        // The second window function pushes the limited query down, so the limit ends up inside a subquery.
        var query = DbContext.TestRows
            .Select(r => new { r.Id, RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(r.Id)) })
            .OrderBy(r => r.Id)
            .Take(3)
            .Select(r => new { r.Id, Reversed = EF.Functions.RowNumber(EF.Functions.Over().OrderByDescending(r.RowNumber)) })
            .OrderBy(r => r.Id);

        var result = query.ToList();

        var expectedSequence = TestRows.OrderBy(r => r.Id).Take(3)
            .Select((r, i) => new { r.Id, Reversed = 3L - i });

        Assert.Equal(expectedSequence, result);
    }
}
