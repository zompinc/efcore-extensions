namespace Zomp.EFCore.WindowFunctions.SqlServer.Tests;

public class TestBase : IDisposable
{
    private readonly SqlServerTestDbContext dbContext;

    public TestBase(ITestOutputHelper output)
    {
        dbContext = new SqlServerTestDbContext(output.ToLoggerFactory());
    }

    protected SqlServerTestDbContext DbContext => dbContext;

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
