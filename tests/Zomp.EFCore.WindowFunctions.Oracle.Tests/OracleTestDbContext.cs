using Zomp.EFCore.WindowFunctions.Oracle;

namespace Zomp.EFCore.WindowFunctions.Oracle.Tests;

public class OracleTestDbContext(ILoggerFactory? loggerFactory = null) : TestDbContext(loggerFactory)
{
    private static string ConnectionString { get; } = @"User Id=system;Password=oracle_tests;Data Source=localhost:1521/ORCLCDB;";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseOracle(
            ConnectionString, o => o.UseWindowFunctions());
    }
}
