namespace Zomp.EFCore.WindowFunctions.Sqlite.Tests;

public class TestBase : IDisposable
{
    protected SqliteTestDbContext DbContext { get; } = new SqliteTestDbContext();

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
