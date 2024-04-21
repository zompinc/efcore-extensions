namespace Zomp.EFCore.WindowFunctions.Oracle.Tests;

public class OracleTestDbContext(ILoggerFactory? loggerFactory = null) : TestDbContext(loggerFactory)
{
    private static string ConnectionString { get; } = GetOracleConnectionString();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        _ = optionsBuilder.UseOracle(
            ConnectionString, o => o.UseWindowFunctions());
    }
}
