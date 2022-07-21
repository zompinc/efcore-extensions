namespace Zomp.EFCore.WindowFunctions.Npgsql.Tests;

[Collection(nameof(NpgsqlCollection))]
public class AvgTests : TestBase
{
    private readonly Testing.AvgTests sumTests;

    public AvgTests(ITestOutputHelper output)
        : base(output)
    {
        sumTests = new Testing.AvgTests(DbContext);
    }

    [Fact]
    public void AvgBasic()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            // Need to cast to double or decimal for some reason.
            Avg = EF.Functions.Avg((decimal)r.Id, EF.Functions.Over()),
        });

        var result = query.ToList();

        var avgId = TestFixture.TestRows.Average(r => r.Id);
        var expectedSequence = Enumerable.Range(0, TestFixture.TestRows.Length).Select(_ => (decimal?)avgId);
        Assert.Equal(expectedSequence, result.Select(r => r.Avg), new Testing.DecimalRoundingEqualityComparer(10));
    }

    [Fact]
    public void AvgWithPartition()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            // Need to cast to double or decimal for some reason.
            Avg = EF.Functions.Avg(
                (double)r.Id,
                EF.Functions.Over().PartitionBy(r.Id / 10)),
        });

        var result = query.ToList();

        var groups = TestFixture.TestRows.GroupBy(r => r.Id / 10)
            .ToDictionary(r => r.Key, r => r.Average(s => s.Id));

        var expectedSequence = TestFixture.TestRows.Select(r => (double?)groups[r.Id / 10]);
        Assert.Equal(expectedSequence, result.Select(r => r.Avg));
    }

    [Fact]
    public void AvgDoubleWithPartition() => sumTests.AvgDoubleWithPartition();
}