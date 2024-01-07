﻿namespace Zomp.EFCore.WindowFunctions.Oracle;

/// <summary>
/// Window function extension methods for <see cref="OracleDbContextOptionsBuilder" />.
/// </summary>
public static class OracleDbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Use window functions.
    /// </summary>
    /// <param name="builder">The build being used to configure Postgres.</param>
    /// <returns>The same builder so that further configuration can be chained.</returns>
    public static OracleDbContextOptionsBuilder UseWindowFunctions(
       this OracleDbContextOptionsBuilder builder)
    {
        builder.AddOrUpdateExtension();
        return builder;
    }

    private static OracleDbContextOptionsBuilder AddOrUpdateExtension(
        this OracleDbContextOptionsBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var coreOptionsBuilder = ((IRelationalDbContextOptionsBuilderInfrastructure)builder).OptionsBuilder;
        var extension = coreOptionsBuilder.Options.FindExtension<OracleDbContextOptionsExtension>() ?? new OracleDbContextOptionsExtension();

        ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);
        coreOptionsBuilder.ReplaceService<IRelationalParameterBasedSqlProcessorFactory, WindowFunctionsOracleParameterBasedSqlProcessorFactory>();
        coreOptionsBuilder.ReplaceService<IQuerySqlGeneratorFactory, WindowFunctionsOracleQuerySqlGeneratorFactory>();

        return builder;
    }
}