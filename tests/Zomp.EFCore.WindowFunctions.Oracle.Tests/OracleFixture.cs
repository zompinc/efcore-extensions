namespace Zomp.EFCore.WindowFunctions.SqlServer.Tests;

public class OracleFixture : TestFixture
{
    public async override Task InitializeAsync()
    {
        TestDBContext = new OracleTestDbContext(Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
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