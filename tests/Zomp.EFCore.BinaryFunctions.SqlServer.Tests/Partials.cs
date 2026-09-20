using Zomp.EFCore.BinaryFunctions.SqlServer.Tests;

namespace Zomp.EFCore.BinaryFunctions.Testing;

[Collection(nameof(SqlServerCollection))]
public partial class BinaryTests(ITestOutputHelper output) : TestBase(output) { }
