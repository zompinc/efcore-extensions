namespace Zomp.EFCore.WindowFunctions.Testing;

public partial class SubQueryTests
{
    [Test]
    public async Task RowNumberWithWhere()
    {
        var query = DbContext.TestRows
            .Where(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) == 1);

        var result = query.ToList();

        var expected = TestRows.First();

        await Assert.That(result.Single()).IsEqualTo(expected, TestRowEqualityComparer.Default);
    }

    [Test]
    public async Task RowNumberWithSingle()
    {
        var query = DbContext.TestRows;

        var result = query.Single(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) == 1);

        var expected = TestRows.First();

        await Assert.That(result).IsEqualTo(expected, TestRowEqualityComparer.Default);
    }

    [Test]
    public async Task TwoRowNumberWithWhere()
    {
        var query = DbContext.TestRows
            .Where(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) == 1
            || EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) == 1);

        var result = query.ToList();

        var expected = TestRows.First();

        await Assert.That(result.Single()).IsEqualTo(expected, TestRowEqualityComparer.Default);
    }

    [Test]
    public void NestedWindowFunctions()
    {
        var query = DbContext.TestRows
            .Select(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(
                EF.Functions.Max(t.Id, EF.Functions.Over()))));

        var result = query.ToList();
    }

    [Test]
    public void DoubleNestedWindowFunctions()
    {
        var query = DbContext.TestRows
            .Select(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(
                EF.Functions.Max(
                    EF.Functions.Max(t.Id, EF.Functions.Over()), EF.Functions.Over()))));

        var result = query.ToList();
    }

    [Test]
    public void WindowFunctionsInOrderBy()
    {
        var query = DbContext.TestRows
            .OrderBy(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)));

        var result = query.ToList();
    }

    [Test]
    public void NestedWindowFunctionsInOrderBy()
    {
        var query = DbContext.TestRows
            .OrderBy(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(
                EF.Functions.Max(t.Id, EF.Functions.Over()))));

        var result = query.ToList();
    }

    [Test]
    public void WindowFunctionsInThenBy()
    {
        var query = DbContext.TestRows
            .OrderByDescending(t => t.Id)
            .ThenBy(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)));

        var result = query.ToList();
    }

    [Test]
    public async Task NestedWindowFunctionsInThenBy()
    {
        // The inner max is each row's own id, so the row number ranks by id and only the ThenBy can order a group of ten.
        var query = DbContext.TestRows
            .OrderBy(t => t.Id / 10)
            .ThenByDescending(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(
                EF.Functions.Max(t.Id, EF.Functions.Over().PartitionBy(t.Id)))));

        var result = query.ToList();

        var expectedSequence = TestRows.OrderBy(t => t.Id / 10).ThenByDescending(t => t.Id);

        await Assert.That(result).IsEquivalentTo(expectedSequence, TestRowEqualityComparer.Default, CollectionOrdering.Matching);
    }

    [Test]
    public void NestedWindowFunctionsInWhere()
    {
        var query = DbContext.TestRows
            .Where(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(
                EF.Functions.Max(t.Id, EF.Functions.Over()))) == 1);

        var result = query.ToList();
    }

    [Test]
    public async Task DenseRankWithWhere()
    {
        var query = DbContext.TestRows
            .Where(t => EF.Functions.DenseRank(EF.Functions.Over().OrderBy(t.Id / 10)) == 2);

        var result = query.ToList();

        var expectedSequence = TestRows.Where(t => t.Id / 10 == 1);

        await Assert.That(result).IsEquivalentTo(expectedSequence, TestRowEqualityComparer.Default, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MaxWithWhere()
    {
        var query = DbContext.TestRows
            .Where(t => EF.Functions.Max(t.Id, EF.Functions.Over().PartitionBy(t.Id / 10)) == 23);

        var result = query.ToList();

        var expected = TestRows.Last();

        await Assert.That(result.Single()).IsEqualTo(expected, TestRowEqualityComparer.Default);
    }

    [Test]
    public async Task SelectAndWhere()
    {
        var query = DbContext.TestRows.Select(t => new { t, RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) })
            .Where(w => w.t.Id < 10)
            .Where(w => w.RowNumber == 1);

        var result = query.ToList();

        var expected = TestRows.First();

        await Assert.That(result.Single().t).IsEqualTo(expected, TestRowEqualityComparer.Default);
    }

    [Test]
    public async Task SelectWithWindowFunctionInWhere()
    {
        var part1 = DbContext.TestRows
            .Select(t => new { t, RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) })
            .Where(w => w.RowNumber == 1)
            .Select(z => new { z.RowNumber, z.t.Id });

        var query = DbContext.TestRows.Where(f => part1.Select(z => z.Id).Contains(f.Id));

        var result = query.ToList();

        var expected = TestRows.First();

        await Assert.That(result.Single()).IsEqualTo(expected, TestRowEqualityComparer.Default);
    }

    [Test]
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

    [Test]
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

    [Test]
    public async Task RowNumberWithWhereAfterJoin()
    {
        // After navigation expansion the Where lambda takes EF Core's private TransparentIdentifier struct.
        var query = DbContext.TestRows
            .Join(DbContext.TestRows, l => l.Id, r => r.Id, (l, r) => new { l, r })
            .Where(z => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(z.r.Id)) == 1)
            .Select(z => z.l);

        var result = query.ToList();

        var expected = TestRows.First();

        await Assert.That(result.Single()).IsEqualTo(expected, TestRowEqualityComparer.Default);
    }

    [Test]
    public async Task AsSubQueryNumbersRowsBeforeFiltering()
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

        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task RowNumberWithWhereInsideCorrelatedCollection()
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

        await Assert.That(result.Select(r => r.Id)).IsEquivalentTo(expectedSequence.Select(e => e.Id), CollectionOrdering.Matching);
        await Assert.That(result.Select(r => r.FirstOfTen)).IsEquivalentTo(expectedSequence.Select(e => e.FirstOfTen), CollectionOrdering.Matching);
    }

    [Test]
    public async Task AverageOverWindowFunction()
    {
        // https://github.com/zompinc/efcore-extensions/issues/25
        var query = DbContext.TestRows
            .Select(t => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)));

        var result = query.Average();

        var expected = Enumerable.Range(1, TestRows.Length).Average();

        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task MaxOverWindowFunctionMember()
    {
        var query = DbContext.TestRows
            .Select(t => new { t.Id, RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) });

        var result = query.Max(w => w.RowNumber);

        await Assert.That(result).IsEqualTo(TestRows.Length);
    }

    [Test]
    public async Task GroupByWindowFunction()
    {
        var query = DbContext.TestRows
            .Select(t => new { t.Id, RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) })
            .GroupBy(w => w.RowNumber % 2)
            .Select(g => new { g.Key, Count = g.Count() })
            .OrderBy(g => g.Key);

        var result = query.ToList();

        var expectedSequence = Enumerable.Range(1, TestRows.Length)
            .GroupBy(i => (long)i % 2)
            .Select(g => new { g.Key, Count = g.Count() })
            .OrderBy(g => g.Key);

        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task WhereAfterWindowFunctionProjectionFiltersNumberedRows()
    {
        // LINQ numbers the rows and then filters them. Filtering first restarts the numbering at 1.
        var query = DbContext.TestRows
            .Select(t => new { t.Id, RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) })
            .Where(w => w.Id > 2)
            .OrderBy(w => w.Id);

        var result = query.ToList();

        var expectedSequence = TestRows
            .OrderBy(t => t.Id)
            .Select((t, i) => new { t.Id, RowNumber = i + 1L })
            .Where(w => w.Id > 2);

        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task WhereOnWindowFunctionKeepsItsValue()
    {
        // The projected row number has to be the one that was filtered on, not one recomputed afterwards.
        var query = DbContext.TestRows
            .Select(t => new { t.Id, RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) })
            .Where(w => w.RowNumber == 3);

        var result = query.ToList();

        var expected = new { TestRows.OrderBy(t => t.Id).ElementAt(2).Id, RowNumber = 3L };

        await Assert.That(result.Single()).IsEqualTo(expected);
    }

    [Test]
    public async Task FirstWithPredicateAfterWindowFunctionProjection()
    {
        var query = DbContext.TestRows
            .Select(t => new { t.Id, RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) })
            .OrderBy(w => w.Id);

        var result = query.First(w => w.Id > 2);

        var expected = new { TestRows.OrderBy(t => t.Id).ElementAt(1).Id, RowNumber = 2L };

        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task WindowFunctionAfterTakeSeesOnlyTakenRows()
    {
        // SQL applies the limit after the window function, LINQ before it.
        var query = DbContext.TestRows
            .OrderBy(t => t.Id)
            .Take(3)
            .Select(t => new { t.Id, Max = EF.Functions.Max(t.Id, EF.Functions.Over()) });

        var result = query.ToList();

        var taken = TestRows.OrderBy(t => t.Id).Take(3).ToList();
        var expectedSequence = taken.Select(t => new { t.Id, Max = (int?)taken.Max(x => x.Id) });

        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task WindowFunctionAfterSkipNumbersRemainingRows()
    {
        var query = DbContext.TestRows
            .OrderBy(t => t.Id)
            .Skip(2)
            .Select(t => new { t.Id, RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) });

        var result = query.ToList();

        var expectedSequence = TestRows.OrderBy(t => t.Id).Skip(2)
            .Select((t, i) => new { t.Id, RowNumber = i + 1L });

        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task JoinAfterWindowFunctionProjectionJoinsNumberedRows()
    {
        // The join drops the first row. The rows have to be numbered before that happens.
        var query = DbContext.TestRows
            .Select(t => new { t.Id, RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) })
            .Join(DbContext.TestRows.Where(r => r.Id > 2), l => l.Id, r => r.Id, (l, r) => new { l.Id, l.RowNumber })
            .OrderBy(j => j.Id);

        var result = query.ToList();

        var expectedSequence = TestRows
            .OrderBy(t => t.Id)
            .Select((t, i) => new { t.Id, RowNumber = i + 1L })
            .Where(w => w.Id > 2);

        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task SelectManyAfterWindowFunctionProjectionJoinsNumberedRows()
    {
        var query = DbContext.TestRows
            .Select(t => new { t.Id, RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) })
            .SelectMany(l => DbContext.TestRows.Where(r => r.Id == l.Id && r.Id > 2), (l, r) => new { l.Id, l.RowNumber })
            .OrderBy(j => j.Id);

        var result = query.ToList();

        var expectedSequence = TestRows
            .OrderBy(t => t.Id)
            .Select((t, i) => new { t.Id, RowNumber = i + 1L })
            .Where(w => w.Id > 2);

        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

#if NET10_0_OR_GREATER
    [Test]
    public async Task LeftJoinAfterWindowFunctionProjectionJoinsNumberedRows()
    {
        // A left join keeps every outer row, so it takes several matches per row to disturb the numbering.
        var query = DbContext.TestRows
            .Select(t => new { t.Id, RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(t.Id)) })
            .LeftJoin(DbContext.TestRows, l => l.Id / 10, r => r.Id / 10, (l, r) => new { l.Id, l.RowNumber, InnerId = r!.Id })
            .OrderBy(j => j.Id)
            .ThenBy(j => j.InnerId);

        var result = query.ToList();

        var expectedSequence = TestRows
            .OrderBy(t => t.Id)
            .Select((t, i) => new { t.Id, RowNumber = i + 1L })
            .SelectMany(l => TestRows.Where(r => r.Id / 10 == l.Id / 10), (l, r) => new { l.Id, l.RowNumber, InnerId = r.Id })
            .OrderBy(j => j.Id)
            .ThenBy(j => j.InnerId);

        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }
#endif
}
