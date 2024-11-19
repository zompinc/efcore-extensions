namespace Zomp.EFCore.Testing;

public class TestFixture : IAsyncLifetime
{
    /// <summary>
    /// Gets the test rows.
    /// </summary>
    /// <remarks>
    /// Using Last non null puzzle for testing. Also fill it with other columns to test other types
    /// (https://www.itprotoday.com/sql-server/last-non-null-puzzle).
    /// </remarks>
    public static ImmutableArray<TestRow> TestRows { get; } = CreateTestRows();

    public TestDbContext? TestDBContext { get; set; }

    /// <inheritdoc/>
    public virtual async Task InitializeAsync()
    {
        ArgumentNullException.ThrowIfNull(TestDBContext);
        _ = await TestDBContext.Database.EnsureDeletedAsync();
        _ = await TestDBContext.Database.EnsureCreatedAsync();

        await TestDBContext.AddRangeAsync(TestRows);

        _ = await TestDBContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public virtual async Task DisposeAsync()
    {
        ArgumentNullException.ThrowIfNull(TestDBContext);
        if (!TestDbContext.Settings.PreserveData)
        {
            _ = await TestDBContext.Database.EnsureDeletedAsync();
        }
    }

    private static ImmutableArray<TestRow> CreateTestRows() =>
        [
            new(2, null, new("ab988b94-a7d3-413d-92a0-8a03c47dd0f4"), new(2022, 01, 01), [0, 2]),
            new(3, 10, new("41b1d4c5-e629-4a23-9514-47857e6c5ad2"), new(2022, 01, 02), [0, 3]),
            new(5, -1, new("5a425c36-3a87-4d55-80a0-0f449897daa4"), new(2022, 01, 03), [0, 5]),
            new(7, null, new("a3517ee2-e14c-4c63-8a12-353eee1c01f9"), new(2022, 01, 04), [0, 7]),
            new(11, null, new("2ce77fd7-937d-4041-9b62-47b7d7a7ef09"), new(2022, 01, 05), [0, 11]),
            new(13, -12, new("2b71c0d2-11ac-4f6c-af5e-5c470202e222"), new(2022, 01, 06), [0, 13]),
            new(17, null, new("5c3330cd-a93b-4d6f-b31c-efb735315993"), new(2022, 01, 07), [0, 17]),
            new(19, null, new("e1c5f8f1-18ca-423d-a35e-b9c54f6897d4"), new(2022, 01, 08), [0, 19]),
            new(23, 1759, new("d288c0f2-b2a2-4eef-98cc-1f4726f1277c"), new(2022, 01, 09), [0, 23]),
        ];
}