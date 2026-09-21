namespace Zomp.EFCore.WindowFunctions.Npgsql.Tests;

public class TestBase : IDisposable
{
    protected NpgsqlTestDbContext DbContext { get; } = new NpgsqlTestDbContext();

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
