namespace Zomp.EFCore.WindowFunctions.Npgsql.Tests;

[Collection(nameof(NpgsqlCollection))]
public class AnalyticTests : TestBase
{
    private readonly Testing.AnalyticTests analyticTests;

    public AnalyticTests(ITestOutputHelper output)
        : base(output)
    {
        analyticTests = new Testing.AnalyticTests(DbContext);
    }

    [Fact]
    public void LeadBasic() => analyticTests.LeadBasic();
}
