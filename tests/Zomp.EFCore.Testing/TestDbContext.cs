namespace Zomp.EFCore.Testing;

public class TestDbContext(ILoggerFactory? loggerFactory = null) : DbContext
{
    public DbSet<TestRow> TestRows { get; set; } = null!;

    public bool IsSqlServer => Database.ProviderName?.Contains("SqlServer", StringComparison.OrdinalIgnoreCase) ?? false;

    public bool IsPostgreSQL => Database.ProviderName?.Contains("PostgreSQL", StringComparison.OrdinalIgnoreCase) ?? false;

    public bool IsSqlite => Database.ProviderName?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) ?? false;

    protected static string GetSqlServerConnectionString(string databaseName)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddEnvironmentVariables("Zomp_EF_")
            .Build();
        var connectionString = config["Data:ConnectionString"]
            ?? "Server=(LocalDB)\\MsSqlLocalDB;Database={0};Trusted_Connection=True";

        return string.Format(connectionString, databaseName);
    }

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