namespace Zomp.EFCore.WindowFunctions.Testing;

public partial class AnalyticTests
{
    private const int Offset = 2;
    private const int Default = 56;

    [Test]
    public async Task LeadBasic()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Lead(r.Id, Offset, Default, EF.Functions.Over().OrderBy(r.Id)));

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i) => i + Offset >= TestRows.Length ? Default : (int?)TestRows[i + Offset].Id);
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task LeadNullForNullHandling()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Lead(r.Id, Offset, Default, null, EF.Functions.Over().OrderBy(r.Id)));

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i) => i + Offset >= TestRows.Length ? Default : (int?)TestRows[i + Offset].Id);
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task LeadRespectNulls()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Lead(r.Id, Offset, Default, NullHandling.RespectNulls, EF.Functions.Over().OrderBy(r.Id)));

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i) => i + Offset >= TestRows.Length ? Default : (int?)TestRows[i + Offset].Id);
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task LeadIgnoreNulls()
    {
        Skip.When(DbContext.IsSqlite || DbContext.IsPostgreSQL, "SQLite and PostgreSQL have no IGNORE NULLS");

        var query = DbContext.TestRows
        .Select(r => new
        {
            Lead = EF.Functions.Lead(r.Id, Offset, Default, NullHandling.IgnoreNulls, EF.Functions.Over().OrderBy(r.Id)),
            Original = r,
        })
        .OrderBy(r => r.Original)
        .Select(z => z.Lead);

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i) => i + Offset >= TestRows.Length ? Default : (int?)TestRows[i + Offset].Id);
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task LagBasic()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Lag(r.Col1, EF.Functions.Over().OrderBy(r.Id)));

        var result = query.ToList();

        var expectedSequence = ((int?[])[null, .. TestRows.Select(z => z.Col1)])[..^1];
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task LagWithStrings()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Lag(r.Col1.ToString(), EF.Functions.Over().OrderBy(r.Id)));

        var result = query.ToList();

        // Nullable<T>.ToString() returns an empty string for null, and EF Core translates it that way since 9.0.
        var expectedSequence = ((string?[])[null, .. TestRows.Select(z => z.Col1.ToString())])[..^1];
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task LagLastNonNull(bool withDefault)
    {
        Skip.When(DbContext.IsSqlite || DbContext.IsPostgreSQL, "SQLite and PostgreSQL have no IGNORE NULLS");

        Expression<Func<TestRow, int?>> lastNonNullExpr = withDefault
            ? r => EF.Functions.Lag(r.Col1, 0, null, NullHandling.IgnoreNulls, EF.Functions.Over().OrderBy(r.Id))
            : r => EF.Functions.Lag(r.Col1, 0, NullHandling.IgnoreNulls, EF.Functions.Over().OrderBy(r.Id));

        var query = DbContext.TestRows.Select(lastNonNullExpr);

        var result = query.ToList();

        var expectedSequence = TestRows.LastNonNull(r => r.Col1);
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task FirstValueWithPartition()
    {
        var result = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.FirstValue(r.Col1, EF.Functions.Over().PartitionBy(r.Id / 10).OrderBy(r.Id)))
            .ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => TestRows.Where(t => t.Id / 10 == r.Id / 10).OrderBy(t => t.Id).First().Col1);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task LastValueToEndOfPartition()
    {
        var result = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.LastValue(r.Col1, EF.Functions.Over().PartitionBy(r.Id / 10).OrderBy(r.Id).Rows().FromCurrentRow().ToUnbounded()))
            .ToList();

        var expected = TestRows
            .OrderBy(r => r.Id)
            .Select(r => TestRows.Where(t => t.Id / 10 == r.Id / 10).OrderBy(t => t.Id).Last().Col1);

        await Assert.That(result).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task LastValueWithDefaultFrameIsCurrentRow()
    {
        // With ORDER BY the default frame ends at the current row, and Id has no peers.
        var result = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.LastValue(r.Col1, EF.Functions.Over().OrderBy(r.Id)))
            .ToList();

        await Assert.That(result).IsEquivalentTo(TestRows.OrderBy(r => r.Id).Select(r => r.Col1), CollectionOrdering.Matching);
    }

    [Test]
    public async Task FirstValueOfString()
    {
        var result = DbContext.TestRows
            .OrderBy(r => r.Id)
            .Select(r => EF.Functions.FirstValue(r.Id.ToString(), EF.Functions.Over().OrderByDescending(r.Id)))
            .ToList();

        var last = TestRows.Max(r => r.Id).ToString(CultureInfo.InvariantCulture);
        await Assert.That(result).IsEquivalentTo(TestRows.Select(_ => (string?)last), CollectionOrdering.Matching);
    }
}
