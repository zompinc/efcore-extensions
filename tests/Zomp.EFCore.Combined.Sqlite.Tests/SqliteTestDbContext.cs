namespace Zomp.EFCore.Combined.Sqlite.Tests;

public class SqliteTestDbContext(ILoggerFactory? loggerFactory = null) : TestDbContext(loggerFactory)
{
    ////private static readonly SqliteConnection Connection = new("DataSource=:memory:");
    private static readonly SqliteConnection Connection
        = new($"DataSource=Zomp_EfCore_Combined_Tests.db");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        _ = optionsBuilder.UseSqlite(
            Connection,
            sqlOptions => sqlOptions.UseWindowFunctions().UseBinaryFunctions());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Convert https://github.com/dotnet/efcore/issues/15078#issuecomment-475784385
        _ = modelBuilder.Entity<TestRow>().Property(r => r.SomeGuid)
           .HasConversion(
                g => g.ToByteArray(),
                b => new Guid(b));
    }
}