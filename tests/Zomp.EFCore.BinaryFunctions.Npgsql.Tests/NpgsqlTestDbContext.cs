namespace Zomp.EFCore.BinaryFunctions.Npgsql.Tests;
public class NpgsqlTestDbContext : TestDbContext
{
    public NpgsqlTestDbContext(ILoggerFactory? loggerFactory = null)
        : base(loggerFactory)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseNpgsql(
            $"Host=localhost;Database=Zomp_Efcore_BinaryFunctions_Tests;Username=npgsql_tests;Password=npgsql_tests",
            o => o.UseBinaryFunctions());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)))
        {
            property.SetColumnType("timestamp without time zone");
        }
    }
}