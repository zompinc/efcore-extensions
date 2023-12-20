namespace Zomp.EFCore.WindowFunctions.Testing;

public partial class AnalyticTests
{
    [Fact]
    public void LeadBasic()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Lead(r.Id, 2, 56, EF.Functions.Over().OrderBy(r.Id).PartitionBy(r.Id / 10)));

        var result = query.ToList();
    }
}