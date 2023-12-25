namespace Zomp.EFCore.WindowFunctions.Testing;

public partial class AnalyticTests
{
    [Fact]
    public void LeadBasic()
    {
        const int offset = 2;
        const int @default = 56;
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Lead(r.Id, offset, @default, EF.Functions.Over().OrderBy(r.Id).PartitionBy(r.Id / 10)));

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i) => i + offset >= TestRows.Length ? @default : (int?)TestRows[i + offset].Id);
        Assert.Equal(expectedSequence, result);
    }
}