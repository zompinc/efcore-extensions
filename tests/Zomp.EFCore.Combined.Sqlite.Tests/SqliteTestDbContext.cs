namespace Zomp.EFCore.Combined.Sqlite.Tests;

public class SqliteTestDbContext : TestDbContext
{
    ////private static readonly SqliteConnection Connection = new("DataSource=:memory:");
    private static readonly SqliteConnection Connection
        = new($"DataSource=Zomp_Efcore_Combined_Tests.db");

    public SqliteTestDbContext(ILoggerFactory? loggerFactory = null)
        : base(loggerFactory)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite(
            Connection,
            sqlOptions => sqlOptions.UseWindowFunctions().UseBinaryFunctions());
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