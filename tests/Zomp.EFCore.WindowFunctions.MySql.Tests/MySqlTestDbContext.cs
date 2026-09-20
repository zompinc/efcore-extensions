namespace Zomp.EFCore.WindowFunctions.MySql.Tests;

public class MySqlTestDbContext(ILoggerFactory? loggerFactory = null) : TestDbContext(loggerFactory)
{
    private static string ConnectionString { get; } = GetMySqlConnectionString("Zomp_EfCore_WindowFunctions_Tests");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        _ = optionsBuilder.UseMySql(
            ConnectionString,
            ServerVersion.Create(8, 4, 0, Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql),
            o => o.UseWindowFunctions());
    }
}
