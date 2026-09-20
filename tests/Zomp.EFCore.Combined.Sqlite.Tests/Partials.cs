using Zomp.EFCore.Combined.Sqlite.Tests;

namespace Zomp.EFCore.Combined.Testing;

[Collection(nameof(SqliteCollection))]
public partial class CombinedTests(ITestOutputHelper output) : TestBase(output) { }
