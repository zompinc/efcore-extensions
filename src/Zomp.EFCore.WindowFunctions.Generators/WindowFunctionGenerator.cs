namespace Zomp.EFCore.WindowFunctions.Generators;

/// <summary>
/// Generates the <c>DbFunctionsExtensions</c> window function overloads listed in <see cref="FunctionTable"/>.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class WindowFunctionGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
        => context.RegisterPostInitializationOutput(static postInitializationContext =>
        {
            foreach (var function in FunctionTable.All)
            {
                postInitializationContext.AddSource(
                    $"DbFunctionsExtensions.Function.{function.Name}.g.cs",
                    SourceText.From(FunctionRenderer.Render(function), Encoding.UTF8));
            }
        });
}
