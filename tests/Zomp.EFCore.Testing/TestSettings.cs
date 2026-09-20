namespace Zomp.EFCore.Testing;

public class TestSettings
{
    public string? SqlServerConnectionString { get; set; }

    public string? NpgSqlConnectionString { get; set; }

    public string? MySqlConnectionString { get; set; }

    public bool PreserveData { get; set; }
}
