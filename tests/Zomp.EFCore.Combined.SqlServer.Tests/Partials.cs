using Zomp.EFCore.Combined.SqlServer.Tests;

namespace Zomp.EFCore.Combined.Testing;

[Collection(nameof(SqlServerCollection))]
public partial class CombinedTests(ITestOutputHelper output) : TestBase(output) { }
