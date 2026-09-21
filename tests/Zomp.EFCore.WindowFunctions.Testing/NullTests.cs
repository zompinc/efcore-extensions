namespace Zomp.EFCore.WindowFunctions.Testing;

public partial class NullTests
{
    [Test]
    public async Task RowNumberWithOrderingNullCheck()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.RowNumber(EF.Functions.Over().OrderBy(r.Col1 == null ? 1 : 2)));

        var result = query.ToList();

        var expectedSequence = TestRows.Select((_, i) => i + 1);
        await Assert.That(result.Select(r => (int)r)).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MaxWithExpressionNullCheck()
    {
        var query = DbContext.TestRows
        .Select(r => EF.Functions.Max(r.Col1 == null ? r.Id : r.Id - 100, EF.Functions.Over()));

        var result = query.ToList();

        var max = TestRows.Max(r => r.Col1 == null ? r.Id : r.Id - 100);
        var expectedSequence = TestRows.Select(_ => (int?)max);
        await Assert.That(result).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MaxWithPartitionNullCheck()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            Max = EF.Functions.Max(r.Id, EF.Functions.Over().OrderByDescending(r.Id).PartitionBy(r.Col1 == null ? 1 : 2)),
            Original = r,
        }).OrderBy(r => r.Original.Id);

        var result = query.ToList();

        var groups = TestRows.GroupBy(r => r.Col1 == null ? 1 : 2)
            .ToDictionary(g => g.Key, g => g.Max(s => s.Id));

        var expectedSequence = TestRows.Select(r => (int?)groups[r.Col1 == null ? 1 : 2]);
        await Assert.That(result.Select(r => r.Max)).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }
}
