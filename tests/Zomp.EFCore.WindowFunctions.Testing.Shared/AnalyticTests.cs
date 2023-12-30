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
    public void LeadNullForRespectOrIgnoreNulls()
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
        .Select(r => EF.Functions.Lead(r.Id, Offset, Default, Clauses.RespectOrIgnoreNulls.RespectNulls, EF.Functions.Over().OrderBy(r.Id)));

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
            Lead = EF.Functions.Lead(r.Id, Offset, Default, Clauses.RespectOrIgnoreNulls.IgnoreNulls, EF.Functions.Over().OrderBy(r.Id)),
            Original = r,
        })
        .OrderBy(r => r.Original)
        .Select(z => z.Lead);

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i) => i + Offset >= TestRows.Length ? Default : (int?)TestRows[i + Offset].Id);
        Assert.Equal(expectedSequence, result);
    }
}