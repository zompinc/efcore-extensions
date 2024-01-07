namespace Zomp.EFCore.WindowFunctions.SqlServer.Tests;

public class OracleFixture : TestFixture
{
    public async override Task InitializeAsync()
    {
        TestDBContext = new OracleTestDbContext(Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        await RemoveTestTableAsync(TestDBContext);
        await TestDBContext.Database.EnsureCreatedAsync();
        await TestDBContext.AddRangeAsync(TestRows);

        await TestDBContext.SaveChangesAsync();
    }

    public async override Task DisposeAsync()
    {
        await base.DisposeAsync();
        if (TestDBContext is not null)
        {
            await RemoveTestTableAsync(TestDBContext);
        }
    }

    private static async Task RemoveTestTableAsync(TestDbContext dbContext)
    {
        await dbContext.Database.ExecuteSqlAsync($"""
        DECLARE cnt NUMBER;
        BEGIN
          SELECT COUNT(*) INTO cnt FROM user_tables WHERE table_name = '{nameof(TestRows)}';
          IF cnt <> 0 THEN
            EXECUTE IMMEDIATE 'DROP TABLE "{nameof(TestRows)}"';
          END IF;
        END;
        """);
    }
}