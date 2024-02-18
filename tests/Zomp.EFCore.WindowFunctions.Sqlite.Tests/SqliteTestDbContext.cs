namespace Zomp.EFCore.WindowFunctions.Sqlite.Tests;

public class SqliteTestDbContext(ILoggerFactory? loggerFactory = null) : TestDbContext(loggerFactory)
{
    ////private static readonly SqliteConnection Connection = new("DataSource=:memory:");
    private static readonly SqliteConnection Connection
        = new($"DataSource=Zomp_EfCore_WindowFunctions_Tests.db");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite(
            Connection,
            sqlOptions => sqlOptions.UseWindowFunctions());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Convert https://github.com/dotnet/efcore/issues/15078#issuecomment-475784385
        modelBuilder.Entity<TestRow>().Property(r => r.SomeGuid)
           .HasConversion(
                g => g.ToByteArray(),
                b => new Guid(b));
    }
}