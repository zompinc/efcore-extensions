namespace Zomp.EFCore.WindowFunctions.MySql.Tests;

public sealed class MySqlFixture : TestFixture
{
    public override async Task InitializeAsync()
    {
        TestDBContext = new MySqlTestDbContext();
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