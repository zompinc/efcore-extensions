using Zomp.EFCore.Combined.Npgsql.Tests;

namespace Zomp.EFCore.Combined.Testing;

[Collection(nameof(NpgsqlCollection))]
public partial class CombinedTests(ITestOutputHelper output) : TestBase(output) { }
