namespace Zomp.EFCore.BinaryFunctions.Npgsql.Tests;

public sealed class NpgsqlFixture : TestFixture
{
    public async override Task InitializeAsync()
    {
        TestDBContext = new NpgsqlTestDbContext();
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