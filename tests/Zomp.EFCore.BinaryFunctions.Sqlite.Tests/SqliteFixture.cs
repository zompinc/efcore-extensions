namespace Zomp.EFCore.BinaryFunctions.Sqlite.Tests;

public sealed class SqliteFixture : TestFixture
{
    public override async Task InitializeAsync()
    {
        TestDBContext = new SqliteTestDbContext();
        await base.InitializeAsync();
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
        if (TestDBContext is not null)
        {
            await TestDBContext.DisposeAsync();
        }
    }
}