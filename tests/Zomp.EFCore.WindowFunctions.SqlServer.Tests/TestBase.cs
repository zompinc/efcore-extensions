namespace Zomp.EFCore.WindowFunctions.SqlServer.Tests;

public class TestBase(ITestOutputHelper output) : IDisposable
{
    protected SqlServerTestDbContext DbContext { get; } = new SqlServerTestDbContext(output.ToLoggerFactory());

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DbContext?.Dispose();
        }
    }
}
