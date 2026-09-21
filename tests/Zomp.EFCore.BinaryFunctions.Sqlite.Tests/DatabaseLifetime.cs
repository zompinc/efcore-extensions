[assembly: NotInParallel]

namespace Zomp.EFCore.BinaryFunctions.Sqlite.Tests;

/// <summary>
/// Creates the test database before the first test of this assembly and deletes it after the last one.
/// </summary>
/// <remarks>
/// The tests share the database and the SQLite ones one connection, so they run one at a time.
/// </remarks>
public static class DatabaseLifetime
{
    private static readonly SqliteFixture Fixture = new();

    [Before(HookType.Assembly)]
    public static Task CreateDatabaseAsync() => Fixture.InitializeAsync();

    [After(HookType.Assembly)]
    public static Task DeleteDatabaseAsync() => Fixture.DisposeAsync();
}
