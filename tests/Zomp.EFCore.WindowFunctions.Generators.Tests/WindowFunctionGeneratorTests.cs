namespace Zomp.EFCore.WindowFunctions.Generators.Tests;

/// <summary>
/// Snapshots of the overloads <see cref="WindowFunctionGenerator"/> writes, one file per window function.
/// </summary>
public class WindowFunctionGeneratorTests
{
    [Test]
    public async Task GeneratesTheOverloads()
    {
        var driver = CSharpGeneratorDriver
            .Create(new WindowFunctionGenerator())
            .RunGenerators(CSharpCompilation.Create("Snapshot"));

        var files = driver.GetRunResult().GeneratedTrees
            .OrderBy(t => t.FilePath, StringComparer.Ordinal)
            .Select(t => new Target("cs", t.GetText().ToString(), Path.GetFileNameWithoutExtension(t.FilePath)));

        await Verify(files);
    }
}
