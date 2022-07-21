namespace Zomp.EFCore.BinaryFunctions.Sqlite;

/// <summary>
/// Window function extension methods for <see cref="SqliteDbContextOptionsBuilderExtensions" />.
/// </summary>
public static class SqliteDbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Use window functions.
    /// </summary>
    /// <param name="builder">The build being used to configure Postgres.</param>
    /// <returns>The same builder so that further configuration can be chained.</returns>
    public static SqliteDbContextOptionsBuilder UseBinaryFunctions(
        this SqliteDbContextOptionsBuilder builder)
    {
        return builder.AddOrUpdateExtension();
    }

    private static SqliteDbContextOptionsBuilder AddOrUpdateExtension(
        this SqliteDbContextOptionsBuilder sqliteOptionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(sqliteOptionsBuilder);

        var coreOptionsBuilder = ((IRelationalDbContextOptionsBuilderInfrastructure)sqliteOptionsBuilder).OptionsBuilder;
        var extension = coreOptionsBuilder.Options.FindExtension<SqliteDbContextOptionsExtension>() ?? new SqliteDbContextOptionsExtension();

        ((IDbContextOptionsBuilderInfrastructure)coreOptionsBuilder).AddOrUpdateExtension(extension);
        coreOptionsBuilder.ReplaceService<IRelationalTypeMappingSource, BinarySqliteTypeMappingSource>();
        coreOptionsBuilder.ReplaceService<IBinaryTranslatorPluginFactory, SqliteBinaryTranslatorPluginFactory>();

        return sqliteOptionsBuilder;
    }
}