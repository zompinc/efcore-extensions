using Zomp.EFCore.WindowFunctions.SqlServer.Tests;

namespace Zomp.EFCore.WindowFunctions.Testing;

#pragma warning disable SA1402 // File may only contain a single type
[Collection(nameof(OracleCollection))]
public partial class MaxTests(ITestOutputHelper output) : TestBase(output) { }

[Collection(nameof(OracleCollection))]
public partial class NullTests(ITestOutputHelper output) : TestBase(output) { }

public partial class AvgTests<TResult>(ITestOutputHelper output) : TestBase(output) { }

[Collection(nameof(OracleCollection))]
public partial class AvgTests(ITestOutputHelper output) : AvgTests<int>(output) { }

[Collection(nameof(OracleCollection))]
public partial class RankTests(ITestOutputHelper output) : TestBase(output) { }

public partial class SumTests<TResult>(ITestOutputHelper output) : TestBase(output) { }

[Collection(nameof(OracleCollection))]
public partial class SumTests(ITestOutputHelper output) : SumTests<int>(output) { }

public partial class CountTests<TResult>(ITestOutputHelper output) : TestBase(output) { }

[Collection(nameof(OracleCollection))]
public partial class CountTests(ITestOutputHelper output) : CountTests<int>(output) { }

[Collection(nameof(OracleCollection))]
public partial class AnalyticTests(ITestOutputHelper output) : TestBase(output) { }
#pragma warning restore SA1402 // File may only contain a single type
