namespace Zomp.EFCore.WindowFunctions.Sqlite.Tests;

[Collection(nameof(SqliteCollection))]
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
