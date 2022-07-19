namespace Zomp.EFCore.BinaryFunctions.Sqlite.Storage.Internal;

/// <summary>
/// Binary type mapping source for SQLite provider.
/// </summary>
public class BinarySqliteTypeMappingSource : SqliteTypeMappingSource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BinarySqliteTypeMappingSource"/> class.
    /// </summary>
    /// <param name="dependencies">The Type Mapping Source Dependencies.</param>
    /// <param name="relationalDependencies">Relational Type Mapping Source Dependencies.</param>
    public BinarySqliteTypeMappingSource(TypeMappingSourceDependencies dependencies, RelationalTypeMappingSourceDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
    {
    }

    /// <inheritdoc/>
    protected override RelationalTypeMapping? FindMapping(in RelationalTypeMappingInfo mappingInfo)
    {
        if (mappingInfo.ClrType is not { } type
            || !type.IsGenericType
            || type.GetGenericTypeDefinition() != typeof(FixedByteArray<>))
        {
            return base.FindMapping(mappingInfo);
        }

        return ((IRelationalTypeMappingSource)this).FindMapping(typeof(byte[]));
    }
}