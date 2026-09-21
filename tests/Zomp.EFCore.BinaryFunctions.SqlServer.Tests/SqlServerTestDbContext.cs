namespace Zomp.EFCore.BinaryFunctions.SqlServer.Tests;

public class SqlServerTestDbContext : TestDbContext
{
    private static string ConnectionString { get; } = GetSqlServerConnectionString("Zomp_EfCore_BinaryFunctions_Tests");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        _ = optionsBuilder.UseSqlServer(
            ConnectionString,
            sqlOptions => sqlOptions.UseBinaryFunctions());
    }
}