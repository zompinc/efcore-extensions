using Zomp.EFCore.BinaryFunctions.Npgsql.Tests;

namespace Zomp.EFCore.BinaryFunctions.Testing;

[Collection(nameof(NpgsqlCollection))]
public partial class BinaryTests(ITestOutputHelper output) : TestBase(output) { }
