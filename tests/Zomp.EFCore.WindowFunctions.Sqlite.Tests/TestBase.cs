namespace Zomp.EFCore.WindowFunctions.Sqlite.Tests;

public class TestBase : IDisposable
{
    private readonly SqliteTestDbContext dbContext;

    public TestBase(ITestOutputHelper output)
    {
        dbContext = new SqliteTestDbContext(output.ToLoggerFactory());
    }

    protected SqliteTestDbContext DbContext => dbContext;

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
