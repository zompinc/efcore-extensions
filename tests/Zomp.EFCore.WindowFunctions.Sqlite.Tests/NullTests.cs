namespace Zomp.EFCore.WindowFunctions.Sqlite.Tests;

[Collection(nameof(SqliteCollection))]
public class NullTests : TestBase
{
    private readonly Testing.NullTests nullTests;

    public NullTests(ITestOutputHelper output)
        : base(output)
    {
        nullTests = new Testing.NullTests(DbContext);
    }

    [Fact]
    public void RowNumberWithOrderingNullCheck() => nullTests.RowNumberWithOrderingNullCheck();

#if EF_CORE_7 || EF_CORE_6
    [Fact(Skip = "Not sure how to set up SqlNullabilityProcessor for older framework, but they are outdated anyways")]
#else
    [Fact]
#endif
    public void MaxWithExpressionNullCheck() => nullTests.MaxWithExpressionNullCheck();

#if EF_CORE_7 || EF_CORE_6
    [Fact(Skip = "Not sure how to set up SqlNullabilityProcessor for older framework, but they are outdated anyways")]
#else
    [Fact]
#endif
    public void MaxWithPartitionNullCheck() => nullTests.MaxWithPartitionNullCheck();
}
