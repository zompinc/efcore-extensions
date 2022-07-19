namespace Zomp.EFCore.Combined.Npgsql.Tests;

public class TestBase : IDisposable
{
    private readonly NpgsqlTestDbContext dbContext;

    public TestBase(ITestOutputHelper output)
    {
        dbContext = new NpgsqlTestDbContext(output.ToLoggerFactory());
    }

    protected NpgsqlTestDbContext DbContext => dbContext;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (dbContext != null)
            {
                dbContext.Dispose();
            }
        }
    }
}
