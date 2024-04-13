namespace Zomp.EFCore.WindowFunctions.Sqlite.Tests;

public class TestBase(ITestOutputHelper output) : IDisposable
{
    protected SqliteTestDbContext DbContext { get; } = new SqliteTestDbContext(output.ToLoggerFactory());

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
