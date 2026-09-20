namespace Zomp.EFCore.WindowFunctions.Oracle.Tests;

public class OracleFixture : TestFixture
{
    public override async Task InitializeAsync()
    {
        TestDBContext = new OracleTestDbContext(Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);
        await RemoveTestTableAsync(TestDBContext);
        _ = await TestDBContext.Database.EnsureCreatedAsync();
        await TestDBContext.AddRangeAsync(TestRows);

        _ = await TestDBContext.SaveChangesAsync();
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
        if (TestDBContext is not null)
        {
            await RemoveTestTableAsync(TestDBContext);
        }
    }

    private static async Task RemoveTestTableAsync(TestDbContext dbContext)
    {
        // Raw on purpose: an interpolated ExecuteSqlAsync turns the table name into a bind parameter,
        // which Oracle does not substitute inside the quoted DROP statement.
        const string Sql = $"""
        DECLARE cnt NUMBER;
        BEGIN
          SELECT COUNT(*) INTO cnt FROM user_tables WHERE table_name = '{nameof(TestRows)}';
          IF cnt <> 0 THEN
            EXECUTE IMMEDIATE 'DROP TABLE "{nameof(TestRows)}"';
          END IF;
        END;
        """;

        _ = await dbContext.Database.ExecuteSqlRawAsync(Sql);
    }
}