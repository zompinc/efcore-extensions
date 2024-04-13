namespace Zomp.EFCore.Testing;

public class TestDbContext(ILoggerFactory? loggerFactory = null) : DbContext
{
    public DbSet<TestRow> TestRows { get; set; } = null!;

    public bool IsSqlServer => Database.ProviderName?.Contains("SqlServer", StringComparison.OrdinalIgnoreCase) ?? false;

    public bool IsPostgreSQL => Database.ProviderName?.Contains("PostgreSQL", StringComparison.OrdinalIgnoreCase) ?? false;

    public bool IsSqlite => Database.ProviderName?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) ?? false;

    internal static TestSettings Settings { get; } = GetSettings();

    protected static string GetNpgsqlConnectionString(string databaseName)
        => GetConnectionString(Settings.NpgSqlConnectionString, "Host=localhost;Database={0};Username=npgsql_tests;Password=npgsql_tests", databaseName);

    protected static string GetSqlServerConnectionString(string databaseName)
        => GetConnectionString(Settings.SqlServerConnectionString, "Server=(LocalDB)\\MsSqlLocalDB;Database={0};Trusted_Connection=True", databaseName);

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => _ = modelBuilder.Entity<TestRow>().Property(x => x.Id).ValueGeneratedNever();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (loggerFactory is not null)
        {
            _ = optionsBuilder.UseLoggerFactory(loggerFactory);
        }
    }

    private static string GetConnectionString(string? connectionTemplate, string defaultTemplate, string databaseName)
    {
        var connectionString = connectionTemplate ?? defaultTemplate;
        return string.Format(connectionString, databaseName);
    }

    private static TestSettings GetSettings()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddEnvironmentVariables("Zomp_EF_")
            .Build();

        var section = config.GetSection("Data");
        var settings = section.Get<TestSettings>() ?? new();

        return settings;
    }
}