namespace Zomp.EFCore.WindowFunctions.Testing;
public class SumTests
{
    private readonly TestDbContext dbContext;

    public SumTests(TestDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public void SimpleSum()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Sum = EF.Functions.Sum(r.Id, EF.Functions.Over()),
        });

        var result = query.ToList();

        var sumId = TestFixture.TestRows.Sum(r => r.Id);
        var expectedSequence = Enumerable.Range(0, TestFixture.TestRows.Length).Select(_ => (int?)sumId);
        Assert.Equal(expectedSequence, result.Select(r => r.Sum));
    }

    public void SumWithPartition()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Sum = EF.Functions.Sum(
                r.Id,
                EF.Functions.Over().PartitionBy(r.Id / 10)),
        });

        var result = query.ToList();

        var groups = TestFixture.TestRows.GroupBy(r => r.Id / 10)
            .ToDictionary(r => r.Key, r => r.Sum(s => s.Id));

        var expectedSequence = TestFixture.TestRows.Select(r => (int?)groups[r.Id / 10]);
        Assert.Equal(expectedSequence, result.Select(r => r.Sum));
    }
}
