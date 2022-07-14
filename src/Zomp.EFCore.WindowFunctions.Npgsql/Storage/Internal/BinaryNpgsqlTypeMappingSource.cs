namespace Zomp.EFCore.WindowFunctions.Npgsql.Storage.Internal;

/// <summary>
/// Binary type mapping source for Postgres provider.
/// </summary>
public class BinaryNpgsqlTypeMappingSource : NpgsqlTypeMappingSource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryNpgsqlTypeMappingSource"/> class.
    /// </summary>
    /// <param name="dependencies">Type mapping source dependencies.</param>
    /// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
    /// <param name="sqlGenerationHelper">sqlGenerationHelper.</param>
    /// <param name="npgsqlOptions">Npgsql Options.</param>
    public BinaryNpgsqlTypeMappingSource(TypeMappingSourceDependencies dependencies, RelationalTypeMappingSourceDependencies relationalDependencies, ISqlGenerationHelper sqlGenerationHelper, INpgsqlSingletonOptions npgsqlOptions)
        : base(dependencies, relationalDependencies, sqlGenerationHelper, npgsqlOptions)
    {
    }

    /*
    protected override RelationalTypeMapping? FindMapping(in RelationalTypeMappingInfo mappingInfo)
    {
        if (mappingInfo.ClrType is not { } type
            || !type.IsGenericType
            || type.GetGenericTypeDefinition() != typeof(FixedByteArray<>))
            return base.FindMapping(mappingInfo);
        return ((IRelationalTypeMappingSource)this).FindMapping(typeof(byte[]));
    }
    */

    // This is to turn an expression into a bit array.
    // Unfortinately I didn't come across a good way of translating bit(n) into bytea

    /// <inheritdoc/>
    protected override RelationalTypeMapping? FindBaseMapping(in RelationalTypeMappingInfo mappingInfo)
    {
        if (mappingInfo.ClrType is not { } type
            || !type.IsGenericType
            || type.GetGenericTypeDefinition() != typeof(FixedByteArray<>))
        {
            return base.FindBaseMapping(mappingInfo);
        }

        var underlyingType = mappingInfo.ClrType.GenericTypeArguments[0];
        var underlyingMapping = FindMapping(underlyingType);

        if (underlyingMapping is null)
        {
            return base.FindBaseMapping(mappingInfo);
        }

        var clrType = underlyingMapping.ClrType;

        var newMappingInfo = mappingInfo with
        {
            ClrType = typeof(BitArray),
            IsFixedLength = true,
            Size = Marshal.SizeOf(clrType) * 8,
        };
        return base.FindBaseMapping(newMappingInfo);
    }
}