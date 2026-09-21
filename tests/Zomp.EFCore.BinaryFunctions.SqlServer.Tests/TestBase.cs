namespace Zomp.EFCore.BinaryFunctions.SqlServer.Tests;

public class TestBase : IDisposable
{
    protected SqlServerTestDbContext DbContext { get; } = new();

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
