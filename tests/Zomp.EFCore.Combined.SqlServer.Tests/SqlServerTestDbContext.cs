namespace Zomp.EFCore.Combined.SqlServer.Tests;

public class SqlServerTestDbContext(ILoggerFactory? loggerFactory = null) : TestDbContext(loggerFactory)
{
    private static string ConnectionString { get; } = GetSqlServerConnectionString("Zomp_EfCore_Combined_Tests");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlServer(
            ConnectionString,
            sqlOptions => sqlOptions.UseWindowFunctions().UseBinaryFunctions());
    }
}