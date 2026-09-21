namespace Zomp.EFCore.Combined.SqlServer.Tests;

public class TestBase : IDisposable
{
    protected SqlServerTestDbContext DbContext { get; } = new SqlServerTestDbContext();

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
