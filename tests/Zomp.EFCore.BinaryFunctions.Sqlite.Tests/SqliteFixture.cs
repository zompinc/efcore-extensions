namespace Zomp.EFCore.BinaryFunctions.Sqlite.Tests;
public class SqliteFixture : TestFixture
{
    public async override Task InitializeAsync()
    {
        TestDBContext = new SqliteTestDbContext();
        await base.InitializeAsync();
    }

    public async override Task DisposeAsync()
    {
        await base.DisposeAsync();
        if (TestDBContext is not null)
        {
            await TestDBContext.DisposeAsync();
        }
    }
}