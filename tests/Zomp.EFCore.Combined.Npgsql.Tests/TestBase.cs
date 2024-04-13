namespace Zomp.EFCore.Combined.Npgsql.Tests;

public class TestBase(ITestOutputHelper output) : IDisposable
{
    protected NpgsqlTestDbContext DbContext { get; } = new NpgsqlTestDbContext(output.ToLoggerFactory());

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
