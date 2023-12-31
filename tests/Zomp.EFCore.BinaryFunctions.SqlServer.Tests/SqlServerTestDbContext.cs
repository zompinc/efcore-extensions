namespace Zomp.EFCore.BinaryFunctions.SqlServer.Tests;

public class SqlServerTestDbContext(ILoggerFactory? loggerFactory = null) : TestDbContext(loggerFactory)
{
    private static string ConnectionString { get; } = GetSqlServerConnectionString("Zomp_EfCore_BinaryFunctions_Tests");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlServer(
            ConnectionString,
            sqlOptions => sqlOptions.UseBinaryFunctions());
    }
}