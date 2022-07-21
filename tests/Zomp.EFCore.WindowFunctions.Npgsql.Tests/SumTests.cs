namespace Zomp.EFCore.WindowFunctions.Npgsql.Tests;

[Collection(nameof(NpgsqlCollection))]
public class SumTests : TestBase
{
    private readonly Testing.SumTests sumTests;

    public SumTests(ITestOutputHelper output)
        : base(output)
    {
        sumTests = new Testing.SumTests(DbContext);
    }

    [Fact]
    public void SimpleSum()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Sum = EF.Functions.Sum((decimal)r.Id, EF.Functions.Over()),
        });

        var result = query.ToList();

        var sumId = TestFixture.TestRows.Sum(r => r.Id);
        var expectedSequence = Enumerable.Range(0, TestFixture.TestRows.Length).Select(_ => (decimal?)sumId);
        Assert.Equal(expectedSequence, result.Select(r => r.Sum));
    }

    [Fact]
    public void SumWithPartition()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Sum = EF.Functions.Sum(
                (decimal)r.Id,
                EF.Functions.Over().PartitionBy(r.Id / 10)),
        });

        var result = query.ToList();

        var groups = TestFixture.TestRows.GroupBy(r => r.Id / 10)
            .ToDictionary(r => r.Key, r => r.Sum(s => s.Id));

        var expectedSequence = TestFixture.TestRows.Select(r => (decimal?)groups[r.Id / 10]);
        Assert.Equal(expectedSequence, result.Select(r => r.Sum));
    }
}