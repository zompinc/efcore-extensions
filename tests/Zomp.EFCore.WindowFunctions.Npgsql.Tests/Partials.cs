using Zomp.EFCore.WindowFunctions.Npgsql.Tests;

namespace Zomp.EFCore.WindowFunctions.Testing;

#pragma warning disable SA1402 // File may only contain a single type
public partial class MaxTests : TestBase { }

public partial class NullTests : TestBase { }

public partial class AvgTests<TResult> : TestBase { }

[InheritsTests]
public partial class AvgTests : AvgTests<decimal> { }

public partial class RankTests : TestBase { }

public partial class SumTests<TResult> : TestBase { }

[InheritsTests]
public partial class SumTests : SumTests<long> { }

public partial class CountTests<TResult> : TestBase { }

[InheritsTests]
public partial class CountTests : CountTests<long> { }

public partial class AnalyticTests : TestBase { }

public partial class SubQueryTests : TestBase { }

public partial class StatisticsTests : TestBase { }

public partial class PagingTests : TestBase { }

public partial class SelectWithIndexTests : TestBase { }

public partial class WhereWithIndexTests : TestBase { }

public partial class DistinctByTests : TestBase { }

public partial class GroupWindowTests : TestBase { }

public partial class TakeWhileTests : TestBase { }

public partial class CorrelatedPartitionTests : TestBase { }
#pragma warning restore SA1402 // File may only contain a single type
