﻿#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Zomp.EFCore.BinaryFunctions.SqlServer;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Window function extension methods for <see cref="SqlServerDbContextOptionsBuilder" />.
/// </summary>
public static class SqlServerDbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Use window functions.
    /// </summary>
    /// <param name="builder">The build being used to configure Postgres.</param>
    /// <returns>The same builder so that further configuration can be chained.</returns>
    public static SqlServerDbContextOptionsBuilder UseBinaryFunctions(
       this SqlServerDbContextOptionsBuilder builder)
    {
        _ = builder.AddOrUpdateExtension();
        return builder;
    }

    private static SqlServerDbContextOptionsBuilder AddOrUpdateExtension(
        this SqlServerDbContextOptionsBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var coreOptionsBuilder = ((IRelationalDbContextOptionsBuilderInfrastructure)builder).OptionsBuilder;
        var extension = coreOptionsBuilder.Options.FindExtension<SqlServerDbContextOptionsExtension>() ?? new SqlServerDbContextOptionsExtension();

        ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);
        _ = coreOptionsBuilder.ReplaceService<IRelationalTypeMappingSource, BinarySqlServerTypeMappingSource>();
        _ = coreOptionsBuilder.ReplaceService<IBinaryTranslatorPluginFactory, SqlServerBinaryTranslatorPluginFactory>();

        return builder;
    }
}