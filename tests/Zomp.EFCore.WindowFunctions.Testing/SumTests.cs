﻿namespace Zomp.EFCore.WindowFunctions.Testing;

public class SumTests(TestDbContext dbContext)
{
    private readonly TestDbContext dbContext = dbContext;

    public void SimpleSum()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Sum = EF.Functions.Sum(r.Id, EF.Functions.Over()),
        });

        var result = query.ToList();

        var sumId = TestRows.Sum(r => r.Id);
        var expectedSequence = Enumerable.Range(0, TestRows.Length).Select(_ => (int?)sumId);
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

        var groups = TestRows.GroupBy(r => r.Id / 10)
            .ToDictionary(r => r.Key, r => r.Sum(s => s.Id));

        var expectedSequence = TestRows.Select(r => (int?)groups[r.Id / 10]);
        Assert.Equal(expectedSequence, result.Select(r => r.Sum));
    }

    public void SumWithPartitionAndOrder()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Sum = EF.Functions.Sum(
                r.Id,
                EF.Functions.Over().OrderBy(r.Id).PartitionBy(r.Id / 10)),
        });

        var result = query.ToList();

        var groups = TestRows.GroupBy(r => r.Id / 10);

        var expectedSequence = TestRows
            .Select(r => groups
                .Where(g => g.Key == r.Id / 10)
                .SelectMany(g => g)
                .Where(z => z.Id <= r.Id)
                .Sum(s => s.Id));

        Assert.Equal(expectedSequence, result.Select(r => r.Sum!.Value));
    }
}
