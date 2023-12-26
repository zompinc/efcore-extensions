namespace Zomp.EFCore.Testing;

public class TestDbContext : DbContext
{
    private readonly ILoggerFactory? loggerFactory;

    public TestDbContext(ILoggerFactory? loggerFactory = null)
    {
        this.loggerFactory = loggerFactory;
    }

    public DbSet<TestRow> TestRows { get; set; } = null!;

    public bool IsSqlServer => Database.ProviderName?.Contains("SqlServer", StringComparison.OrdinalIgnoreCase) ?? false;

    public bool IsPostgreSQL => Database.ProviderName?.Contains("PostgreSQL", StringComparison.OrdinalIgnoreCase) ?? false;

    public bool IsSqlite => Database.ProviderName?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) ?? false;

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestRow>().Property(x => x.Id).ValueGeneratedNever();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (loggerFactory is not null)
        {
            optionsBuilder.UseLoggerFactory(loggerFactory);
        }
    }
}