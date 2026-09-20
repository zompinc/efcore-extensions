using Zomp.EFCore.BinaryFunctions.Sqlite.Tests;

namespace Zomp.EFCore.BinaryFunctions.Testing;

[Collection(nameof(SqliteCollection))]
public partial class BinaryTests(ITestOutputHelper output) : TestBase(output) { }
