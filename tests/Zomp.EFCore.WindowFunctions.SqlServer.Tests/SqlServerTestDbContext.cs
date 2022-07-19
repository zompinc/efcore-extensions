namespace Zomp.EFCore.WindowFunctions.SqlServer.Tests;

public class SqlServerTestDbContext : TestDbContext
{
    public SqlServerTestDbContext(ILoggerFactory? loggerFactory = null)
        : base(loggerFactory)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlServer(
            $@"Server=(localdb)\mssqllocaldb;Database=Zomp_Efcore_WindowFunctions_Tests;Trusted_Connection=True",
            sqlOptions => sqlOptions.UseWindowFunctions().UseBinaryFunctions());
    }
}