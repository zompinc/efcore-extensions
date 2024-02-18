namespace Zomp.EFCore.BinaryFunctions.Npgsql.Tests;

public class NpgsqlTestDbContext(ILoggerFactory? loggerFactory = null) : TestDbContext(loggerFactory)
{
    private static string ConnectionString { get; } = GetNpgsqlConnectionString("Zomp_EfCore_BinaryFunctions_Tests");

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseNpgsql(
            ConnectionString,
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