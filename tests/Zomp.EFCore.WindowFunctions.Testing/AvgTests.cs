namespace Zomp.EFCore.WindowFunctions.Testing;
public class AvgTests
{
    private readonly TestDbContext dbContext;

    public AvgTests(TestDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public void AvgBasic()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Avg = EF.Functions.Avg(r.Id, EF.Functions.Over()),
        });

        var result = query.ToList();

        var avgId = TestFixture.TestRows.Average(r => r.Id);
        var expectedSequence = Enumerable.Range(0, TestFixture.TestRows.Length).Select(_ => (int?)avgId);
        Assert.Equal(expectedSequence, result.Select(r => r.Avg));
    }

    public void AvgWithPartition()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Avg = EF.Functions.Avg(
                r.Id,
                EF.Functions.Over().PartitionBy(r.Id / 10)),
        });

        var result = query.ToList();

        var groups = TestFixture.TestRows.GroupBy(r => r.Id / 10)
            .ToDictionary(r => r.Key, r => r.Average(s => s.Id));

        var expectedSequence = TestFixture.TestRows.Select(r => (int?)groups[r.Id / 10]);
        Assert.Equal(expectedSequence, result.Select(r => r.Avg));
    }

    public void AvgDoubleWithPartition()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
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

    public void AvgNullableWithPartition()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Avg = EF.Functions.Avg(
                (double?)r.Col1,
                EF.Functions.Over().PartitionBy(r.Id / 10)),
        });

        var result = query.ToList();

        var groups = TestFixture.TestRows.GroupBy(r => r.Id / 10)
            .ToDictionary(r => r.Key, r => r.Average(s => s.Col1));

        var expectedSequence = TestFixture.TestRows.Select(r => groups[r.Id / 10]);
        Assert.Equal(expectedSequence, result.Select(r => r.Avg));
    }
}
