namespace Zomp.EFCore.BinaryFunctions.SqlServer.Tests;

public sealed class SqlServerFixture : TestFixture
{
    public override async Task InitializeAsync()
    {
        TestDBContext = new SqlServerTestDbContext(Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
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