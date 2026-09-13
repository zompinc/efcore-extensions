namespace Zomp.EFCore.WindowFunctions.Generators;

/// <summary>
/// Renders the source file holding every overload of one window function.
/// </summary>
internal static class FunctionRenderer
{
    private static readonly ExpressionType[] ExpressionTypes =
    [
        ExpressionType.NonNullableStruct,
        ExpressionType.NullableStruct,
        ExpressionType.ByteArray,
        ExpressionType.String,
        ExpressionType.Generic,
    ];

    private static readonly bool[] BooleanOptions = [false, true];

    public static string Render(FunctionDefinition function)
    {
        var configurations = GetConfigurations(function);

        // Line endings are fixed so the output does not depend on the build machine.
        using var writer = new StringWriter(CultureInfo.InvariantCulture) { NewLine = "\n" };
        writer.WriteLine("namespace Zomp.EFCore.WindowFunctions;");
        writer.WriteLine();
        writer.WriteLine("#nullable enable");
        writer.WriteLine();
        writer.WriteLine("/// <summary>");
        writer.WriteLine("/// Provides extension methods for window functions.");
        writer.WriteLine("/// </summary>");
        writer.WriteLine("public static partial class DbFunctionsExtensions");
        writer.WriteLine("{");

        for (var i = 0; i < configurations.Count; ++i)
        {
            var configuration = configurations[i];
            writer.WriteLine("    /// <summary>");
            writer.WriteLine($"    /// {function.Summary}");
            writer.WriteLine("    /// </summary>");
            if (function.FunctionType != FunctionType.NoArguments)
            {
                if (configuration.IsGeneric)
                {
                    writer.WriteLine("    /// <typeparam name=\"T\">Type of object.</typeparam>");
                }

                if (configuration.CustomReturnType)
                {
                    writer.WriteLine("    /// <typeparam name=\"TResult\">Type of the result object.</typeparam>");
                }
            }

            writer.WriteLine("    /// <param name=\"_\">The <see cref=\"DbFunctions\"/> instance.</param>");
            writer.WriteLine(string.Join(writer.NewLine, configuration.ParameterTrivia));
            writer.WriteLine("    /// <param name=\"over\">over clause.</param>");
            writer.WriteLine($"    /// <returns>{function.Name} for the selected window frame.</returns>");
            writer.WriteLine("    /// <exception cref=\"InvalidOperationException\">Occurs on client-side evaluation.</exception>");
            var nullableSuffix = function.NonNullableReturnType ? string.Empty : "?";
            writer.WriteLine($"    public static {configuration.ReturnType}{nullableSuffix} {function.Name}{configuration.GenericTypeParameters}(this DbFunctions _, {configuration.Arguments}OverClause over)");
            if (configuration.IsGenericWithQualifier)
            {
                writer.WriteLine("        where T : struct");
            }

            writer.WriteLine($"        => throw new InvalidOperationException(CoreStrings.FunctionOnClient(nameof({function.Name}))  + UseWindowFunctions);");
            if (i != configurations.Count - 1)
            {
                writer.WriteLine();
            }
        }

        writer.Write("}");
        return writer.ToString();
    }

    private static List<Configuration> GetConfigurations(FunctionDefinition function)
    {
        var configurations = new List<Configuration>();
        var structsOnly = function.VariationType == VariationType.StructsOnly;
        var maxOptionalArguments = function.FunctionType == FunctionType.LeadLag ? 2 : 0;

        for (var numberOfOptionalArguments = 0; numberOfOptionalArguments <= maxOptionalArguments; ++numberOfOptionalArguments)
        {
            foreach (var respectIgnoreNullsOption in BooleanOptions)
            {
                if (function.FunctionType != FunctionType.LeadLag && respectIgnoreNullsOption)
                {
                    continue;
                }

                foreach (var customReturnTypeOption in BooleanOptions)
                {
                    if (!function.CustomReturnType && customReturnTypeOption)
                    {
                        continue;
                    }

                    foreach (var expressionType in ExpressionTypes)
                    {
                        var configuration = new Configuration(function.FunctionType, expressionType, customReturnTypeOption, function.SpecificReturnType, respectIgnoreNullsOption, numberOfOptionalArguments);

                        if (structsOnly && !configuration.IsGeneric)
                        {
                            continue;
                        }

                        if ((function.VariationType == VariationType.Generic) ^ (expressionType == ExpressionType.Generic))
                        {
                            continue;
                        }

                        configurations.Add(configuration);
                    }
                }
            }
        }

        return configurations;
    }
}
