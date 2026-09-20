namespace Zomp.EFCore.WindowFunctions.MySql.Tests;

public class TestBase(ITestOutputHelper output) : IDisposable
{
    protected MySqlTestDbContext DbContext { get; } = new MySqlTestDbContext(output.ToLoggerFactory());

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
