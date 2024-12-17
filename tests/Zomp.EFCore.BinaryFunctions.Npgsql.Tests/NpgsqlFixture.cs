namespace Zomp.EFCore.BinaryFunctions.Npgsql.Tests;

public sealed class NpgsqlFixture : TestFixture
{
    public override async Task InitializeAsync()
    {
        TestDBContext = new NpgsqlTestDbContext();
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