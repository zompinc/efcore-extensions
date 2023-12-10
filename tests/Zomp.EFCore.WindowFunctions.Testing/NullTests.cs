﻿namespace Zomp.EFCore.WindowFunctions.Testing;

public class NullTests(TestDbContext dbContext)
{
    public void RowNumberWithOrderingNullCheck()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(r.Col1 == null ? 1 : 2)),
        });

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i) => i + 1);
        Assert.Equal(expectedSequence, result.Select(r => (int)r.RowNumber));
    }

    public void MaxWithExpressionNullCheck()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.Col1 == null ? r.Id : r.Id - 100, EF.Functions.Over()),
        });

        var result = query.ToList();

        var max = TestRows.Max(r => r.Col1 == null ? r.Id : r.Id - 100);
        var expectedSequence = TestRows.Select(_ => (int?)max);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }

    public void MaxWithPartitionNullCheck()
    {
        var query = dbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.Id, EF.Functions.Over().OrderByDescending(r.Id).PartitionBy(r.Col1 == null ? 1 : 2)),
            Original = r,
        }).OrderBy(r => r.Original.Id);

        var result = query.ToList();

        var groups = TestRows.GroupBy(r => r.Col1 == null ? 1 : 2)
            .ToDictionary(g => g.Key, g => g.Max(s => s.Id));

        var expectedSequence = TestRows.Select(r => (int?)groups[r.Col1 == null ? 1 : 2]);
        Assert.Equal(expectedSequence, result.Select(r => r.Max));
    }
}
