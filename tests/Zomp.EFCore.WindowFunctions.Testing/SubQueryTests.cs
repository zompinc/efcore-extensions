namespace Zomp.EFCore.WindowFunctions.Testing;

public partial class SubQueryTests
{
    [Fact]
    public void RowNumberWithWhere()
    {
        var query = DbContext.TestRows
            .Where(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) == 1);

        var result = query.ToList();

        var expected = TestRows.First();

        Assert.Equal(expected, result.Single(), TestRowEqualityComparer.Default);
    }

    [SkippableFact]
    public void RowNumberWithSingle()
    {
        // Fixme: investigate why this fails.
        Skip.If(DbContext.IsSqlite);
        var result = DbContext.TestRows
            .Single(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) == 1);

        var expected = TestRows.First();

        Assert.Equal(expected, result, TestRowEqualityComparer.Default);
    }

    [Fact]
    public void TwoRowNumberWithWhere()
    {
        var query = DbContext.TestRows
            .Where(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) == 1
            || EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) == 1);

        var result = query.ToList();

        var expected = TestRows.First();

        Assert.Equal(expected, result.Single(), TestRowEqualityComparer.Default);
    }

    [Fact]
    public void NestedWindowFunctions()
    {
        var query = DbContext.TestRows
            .Select(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(
                EF.Functions.Max(t.Id, EF.Functions.Over()))));

        var result = query.ToList();
    }

    [Fact]
    public void DoubleNestedWindowFunctions()
    {
        var query = DbContext.TestRows
            .Select(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(
                EF.Functions.Max(
                    EF.Functions.Max(t.Id, EF.Functions.Over()), EF.Functions.Over()))));

        var result = query.ToList();
    }

    [Fact]
    public void WindowFunctionsInOrderBy()
    {
        var query = DbContext.TestRows
            .OrderBy(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)));

        var result = query.ToList();
    }

    [Fact]
    public void NestedWindowFunctionsInOrderBy()
    {
        var query = DbContext.TestRows
            .OrderBy(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(
                EF.Functions.Max(t.Id, EF.Functions.Over()))));

        var result = query.ToList();
    }

    [Fact]
    public void WindowFunctionsInThenBy()
    {
        var query = DbContext.TestRows
            .OrderByDescending(t => t.Id)
            .ThenBy(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)));

        var result = query.ToList();
    }

    [Fact(Skip = "Need to implement")]
    public void NestedWindowFunctionsInThenBy()
    {
        var query = DbContext.TestRows
            .OrderByDescending(t => t.Id)
            .ThenBy(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(
                EF.Functions.Max(t.Id, EF.Functions.Over()))));

        var result = query.ToList();
    }

    [Fact]
    public void NestedWindowFunctionsInWhere()
    {
        var query = DbContext.TestRows
            .Where(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(
                EF.Functions.Max(t.Id, EF.Functions.Over()))) == 1);

        var result = query.ToList();
    }

    [Fact]
    public void DenseRankWithWhere()
    {
        var query = DbContext.TestRows
            .Where(t => EF.Functions.DenseRank(EF.Functions.Over().OrderBy(t.Id / 10)) == 2);

        var result = query.ToList();

        var expectedSequence = TestRows.Where(t => t.Id / 10 == 1);

        Assert.Equal(expectedSequence, result, TestRowEqualityComparer.Default);
    }

    [Fact]
    public void MaxWithWhere()
    {
        var query = DbContext.TestRows
            .Where(t => EF.Functions.Max(t.Id, EF.Functions.Over().PartitionBy(t.Id / 10)) == 23);

        var result = query.ToList();

        var expected = TestRows.Last();

        Assert.Equal(expected, result.Single(), TestRowEqualityComparer.Default);
    }

    [Fact]
    public void SelectAndWhere()
    {
        var query = DbContext.TestRows.Select(t => new { t, RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) })
            .Where(w => w.t.Id < 10)
            .Where(w => w.RowNumber == 1);

        var result = query.ToList();

        var expected = TestRows.First();

        Assert.Equal(expected, result.Single().t, TestRowEqualityComparer.Default);
    }

    [Fact(Skip = "Should work, but doesn't")]
    public void SelectWithWindowFunctionInWhere()
    {
        var part1 = DbContext.TestRows
            .Select(t => new { t, RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) })
            .Where(w => w.RowNumber == 1)
            .Select(z => new { z.RowNumber, z.t.Id });

        var query = DbContext.TestRows.Where(f => part1.Select(z => z.Id).Contains(f.Id));

        var result = query.ToList();
    }

    [Fact]
    public void Join()
    {
        var query = DbContext.TestRows.Join(
            DbContext.TestRows.Select(subRow => new
            {
                subRow.Id,
                RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(subRow.Date).PartitionBy(subRow.Col1)),
            }),
            l => l.Id,
            r => r.Id,
            (l, r) => new { r.Id, r.RowNumber });

        var queryStr = query.ToQueryString();
    }

    [Fact]
    public void WhereDoesNotAffectPrecedingJoin()
    {
        var query = DbContext.TestRows.Join(
            DbContext.TestRows.Select(subRow => new
            {
                subRow.Id,
                RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(subRow.Date).PartitionBy(subRow.Col1)),
            }),
            l => l.Id,
            r => r.Id,
            (l, r) => new { r.Id, r.RowNumber })
            .Where(w => w.Id != -999);

        var queryStr = query.ToQueryString();
    }
}
