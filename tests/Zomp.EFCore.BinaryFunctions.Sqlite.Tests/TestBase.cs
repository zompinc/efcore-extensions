namespace Zomp.EFCore.BinaryFunctions.Sqlite.Tests;

public class TestBase : IDisposable
{
    protected SqliteTestDbContext DbContext { get; } = new();

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
