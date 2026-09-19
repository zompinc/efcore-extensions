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

    [Fact]
    public void RowNumberWithWhereAfterJoin()
    {
        // After navigation expansion the Where lambda takes EF Core's private TransparentIdentifier struct.
        var query = DbContext.TestRows
            .Join(DbContext.TestRows, l => l.Id, r => r.Id, (l, r) => new { l, r })
            .Where(z => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(z.r.Id)) == 1)
            .Select(z => z.l);

        var result = query.ToList();

        var expected = TestRows.First();

        Assert.Equal(expected, result.Single(), TestRowEqualityComparer.Default);
    }

    [Fact]
    public void AsSubQueryNumbersRowsBeforeFiltering()
    {
        // The filter has no window function in it, so nothing is pushed down without the hint.
        var query = DbContext.TestRows
            .Select(t => new { t.Id, RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) })
            .AsSubQuery()
            .Where(w => w.Id > 2)
            .OrderBy(w => w.Id);

        var result = query.ToList();

        var expectedSequence = TestRows
            .OrderBy(t => t.Id)
            .Select((t, i) => new { t.Id, RowNumber = i + 1L })
            .Where(w => w.Id > 2);

        Assert.Equal(expectedSequence, result);
    }

    [Fact]
    public void RowNumberWithWhereInsideCorrelatedCollection()
    {
        // EF Core translates the inner query with a visitor it gets from CreateSubqueryVisitor.
        // The correlation comes after the window function so that SQLite doesn't need APPLY.
        var query = DbContext.TestRows
            .OrderBy(outer => outer.Id)
            .Select(outer => new
            {
                outer.Id,
                FirstOfTen = DbContext.TestRows
                    .Where(inner => EF.Functions.RowNumber(EF.Functions.Over().PartitionBy(inner.Id / 10).OrderBy(inner.Id)) == 1)
                    .Where(inner => inner.Id == outer.Id)
                    .Select(inner => inner.Id)
                    .ToList(),
            });

        var result = query.ToList();

        var firstOfTens = TestRows.GroupBy(t => t.Id / 10).Select(g => g.Min(t => t.Id)).ToHashSet();
        var expectedSequence = TestRows.OrderBy(t => t.Id)
            .Select(t => new { t.Id, FirstOfTen = firstOfTens.Contains(t.Id) ? new List<int> { t.Id } : [] });

        Assert.Equal(expectedSequence.Select(e => e.Id), result.Select(r => r.Id));
        Assert.Equal(expectedSequence.Select(e => e.FirstOfTen), result.Select(r => r.FirstOfTen));
    }
}
