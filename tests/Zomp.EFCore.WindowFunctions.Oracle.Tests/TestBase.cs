namespace Zomp.EFCore.WindowFunctions.Oracle.Tests;

public class TestBase(ITestOutputHelper output) : IDisposable
{
    protected OracleTestDbContext DbContext { get; } = new OracleTestDbContext(output.ToLoggerFactory());

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
