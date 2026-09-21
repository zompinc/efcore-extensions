namespace Zomp.EFCore.WindowFunctions.Generators;

/// <summary>
/// The window functions to generate.
/// </summary>
internal static class FunctionTable
{
    public static IReadOnlyList<FunctionDefinition> All { get; } =
    [
        new("Max", "The MAX() window function returns the maximum value of the expression across all input values."),
        new("Min", "The MIN() window function returns the minimum value of the expression across all input values."),
        new("Avg", "The AVG() window function returns the average value of the expression across all input values.", variationType: VariationType.StructsOnly, customReturnType: true),
        new("Sum", "The SUM() window function returns the sum value of the expression across all input values.", variationType: VariationType.StructsOnly, customReturnType: true),
        new("Count", "The COUNT() window function returns the count of the expression across all input values.", variationType: VariationType.Generic, customReturnType: true, nonNullableReturnType: true, specificReturnType: "int"),
        new("RowNumber", "The ROW_NUMBER() window function returns the row number value of the expression across all input values.", FunctionType.NoArguments, VariationType.Generic, nonNullableReturnType: true, specificReturnType: "long"),
        new("Rank", "The RANK() window function returns the rank value of the expression across all input values.", FunctionType.NoArguments, VariationType.Generic, nonNullableReturnType: true, specificReturnType: "long"),
        new("DenseRank", "The DENSE_RANK() window function returns the dense rank value of the expression across all input values.", FunctionType.NoArguments, VariationType.Generic, nonNullableReturnType: true, specificReturnType: "long"),
        new("PercentRank", "The PERCENT_RANK() window function returns the percent rank value of the expression across all input values.", FunctionType.NoArguments, VariationType.Generic, nonNullableReturnType: true, specificReturnType: "double"),
        new("Lead", "The LEAD window function returns the values for a row at a given offset below (after) the current row in the partition.", FunctionType.LeadLag),
        new("Lag", "The LAG window function returns the values for a row at a given offset above (before) the current row in the partition.", FunctionType.LeadLag),
        new("StandardDeviationSample", "The sample standard deviation window function (STDEV on SQL Server, STDDEV_SAMP elsewhere) returns the sample standard deviation of the expression across all input values.", variationType: VariationType.StructsOnly, specificReturnType: "double"),
        new("StandardDeviationPopulation", "The population standard deviation window function (STDEVP on SQL Server, STDDEV_POP elsewhere) returns the population standard deviation of the expression across all input values.", variationType: VariationType.StructsOnly, specificReturnType: "double"),
        new("VarianceSample", "The sample variance window function (VAR on SQL Server, VAR_SAMP elsewhere) returns the sample variance of the expression across all input values.", variationType: VariationType.StructsOnly, specificReturnType: "double"),
        new("VariancePopulation", "The population variance window function (VARP on SQL Server, VAR_POP elsewhere) returns the population variance of the expression across all input values.", variationType: VariationType.StructsOnly, specificReturnType: "double"),
    ];
}
