namespace Zomp.EFCore.Combined.Testing;

public partial class CombinedTests
{
    [Test]
    public async Task LastNonNullArithmetic()
    {
        var query = DbContext.TestRows
        .Select(r => new
        {
            LastNonNull =
            EF.Functions.BinaryCast<long, int>(
                EF.Functions.Max(
                    r.Col1.HasValue ? (r.Id * (1L << 32)) | (r.Col1.Value & uint.MaxValue) : (long?)null,
                    EF.Functions.Over().OrderBy(r.Id))),
        });

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows.LastNonNull(r => r.Col1);
        await Assert.That(result.Select(r => r.LastNonNull)).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task LastNonNull()
    {
        Skip.When(DbContext.IsSqlite, "SQLite has nothing that reads an integer back out of a blob: CAST treats the bytes as text");

        var query = DbContext.TestRows
        .Select(r => new
        {
            LastNonNull =
            EF.Functions.ToValue<int>(
                EF.Functions.Substring(
                    EF.Functions.Max(
                        EF.Functions.Concat(
                            EF.Functions.GetBytes(r.Id), EF.Functions.GetBytes(r.Col1)),
                        EF.Functions.Over().OrderBy(r.Id)),
                    5,
                    4)),
        });

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows.LastNonNull(r => r.Col1);
        await Assert.That(result.Select(r => r.LastNonNull)).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }

    [Test]
    public async Task LastNonNullShorthand()
    {
        Skip.When(DbContext.IsSqlite, "SQLite has nothing that reads an integer back out of a blob: CAST treats the bytes as text");

        var query = DbContext.TestRows
        .Select(r => new
        {
            LastNonNull =
            EF.Functions.ToValue<int>(
                EF.Functions.Max(
                    EF.Functions.Concat(
                        EF.Functions.GetBytes(r.Id), EF.Functions.GetBytes(r.Col1)),
                    EF.Functions.Over().OrderBy(r.Id)),
                5),
        });

        var result = query.ToList();

        var expectedSequence = TestFixture.TestRows.LastNonNull(r => r.Col1);
        await Assert.That(result.Select(r => r.LastNonNull)).IsEquivalentTo(expectedSequence, CollectionOrdering.Matching);
    }
}
