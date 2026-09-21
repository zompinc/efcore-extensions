namespace Zomp.EFCore.Combined.SqlServer.Tests;

public class SqlServerTestDbContext : TestDbContext
{
    private static string ConnectionString { get; } = GetSqlServerConnectionString("Zomp_EfCore_Combined_Tests");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        _ = optionsBuilder.UseSqlServer(
            ConnectionString,
            sqlOptions => sqlOptions.UseWindowFunctions().UseBinaryFunctions());
    }
}