namespace Zomp.EFCore.WindowFunctions.Testing;

public class TestDbContext : DbContext
{
    private readonly ILoggerFactory? loggerFactory;

    public TestDbContext(ILoggerFactory? loggerFactory = null)
    {
        this.loggerFactory = loggerFactory;
    }

    public DbSet<TestRow> TestRows { get; set; } = null!;

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