namespace Zomp.EFCore.WindowFunctions.Testing;

public partial class AnalyticTests
{
    private const int Offset = 2;
    private const int Default = 56;

    [Fact]
    public void LeadBasic()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Lead(r.Id, Offset, Default, EF.Functions.Over().OrderBy(r.Id)));

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i) => i + Offset >= TestRows.Length ? Default : (int?)TestRows[i + Offset].Id);
        Assert.Equal(expectedSequence, result);
    }

    [SkippableFact]
    public void LeadNullForNullHandling()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Lead(r.Id, Offset, Default, null, EF.Functions.Over().OrderBy(r.Id)));

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i) => i + Offset >= TestRows.Length ? Default : (int?)TestRows[i + Offset].Id);
        Assert.Equal(expectedSequence, result);
    }

    [SkippableFact]
    public void LeadRespectNulls()
    {
        Skip.If(DbContext.IsSqlite || DbContext.IsPostgreSQL);

        var query = DbContext.TestRows
        .Select(r => EF.Functions.Lead(r.Id, Offset, Default, NullHandling.RespectNulls, EF.Functions.Over().OrderBy(r.Id)));

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i) => i + Offset >= TestRows.Length ? Default : (int?)TestRows[i + Offset].Id);
        Assert.Equal(expectedSequence, result);
    }

    [SkippableFact]
    public void LeadIgnoreNulls()
    {
        Skip.If(DbContext.IsSqlite || DbContext.IsPostgreSQL);

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
        Assert.Equal(expectedSequence, result);
    }

    [Fact]
    public void LagBasic()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Lag(r.Col1, EF.Functions.Over().OrderBy(r.Id)));

        var result = query.ToList();

        var expectedSequence = ((int?[])[null, .. TestRows.Select(z => z.Col1)])[..^1];
        Assert.Equal(expectedSequence, result);
    }

    [Fact(Skip = "EF Core 9 changed things, look into this")]
    public void LagWithStrings()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Lag(r.Col1.ToString(), EF.Functions.Over().OrderBy(r.Id)));

        var result = query.ToList();

        var expectedSequence = ((string?[])[null, .. TestRows.Select(z => z.Col1?.ToString())])[..^1];
        Assert.Equal(expectedSequence, result);
    }

    [SkippableTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void LagLastNonNull(bool withDefault)
    {
        Skip.If(DbContext.IsSqlite || DbContext.IsPostgreSQL);

        Expression<Func<TestRow, int?>> lastNonNullExpr = withDefault
            ? r => EF.Functions.Lag(r.Col1, 0, null, NullHandling.IgnoreNulls, EF.Functions.Over().OrderBy(r.Id))
            : r => EF.Functions.Lag(r.Col1, 0, NullHandling.IgnoreNulls, EF.Functions.Over().OrderBy(r.Id));

        var query = DbContext.TestRows.Select(lastNonNullExpr);

        var result = query.ToList();

        var expectedSequence = TestRows.LastNonNull(r => r.Col1);
        Assert.Equal(expectedSequence, result);
    }
}