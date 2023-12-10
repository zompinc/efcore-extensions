namespace Zomp.EFCore.WindowFunctions.Npgsql.Tests;

[Collection(nameof(NpgsqlCollection))]
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

    [Fact]
    public void MaxWithExpressionNullCheck() => nullTests.MaxWithExpressionNullCheck();

    [Fact]
    public void MaxWithPartitionNullCheck() => nullTests.MaxWithPartitionNullCheck();
}
