namespace Zomp.EFCore.BinaryFunctions.Sqlite.Storage.Internal;

/// <summary>
/// Binary type mapping source for SQLite provider.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BinarySqliteTypeMappingSource"/> class.
/// </remarks>
/// <param name="dependencies">The Type Mapping Source Dependencies.</param>
/// <param name="relationalDependencies">Relational Type Mapping Source Dependencies.</param>
public class BinarySqliteTypeMappingSource(TypeMappingSourceDependencies dependencies, RelationalTypeMappingSourceDependencies relationalDependencies) : SqliteTypeMappingSource(dependencies, relationalDependencies)
{
    /// <inheritdoc/>
    protected override RelationalTypeMapping? FindMapping(in RelationalTypeMappingInfo mappingInfo)
        => mappingInfo.ClrType is not { } type
            || !type.IsGenericType
            || type.GetGenericTypeDefinition() != typeof(FixedByteArray<>)
            ? base.FindMapping(mappingInfo)
            : ((IRelationalTypeMappingSource)this).FindMapping(typeof(byte[]));
}