namespace Zomp.EFCore.BinaryFunctions.SqlServer.Storage.Internal;

/// <summary>
/// Binary type mapping source for SQL Server provider.
/// </summary>
public class BinarySqlServerTypeMappingSource : SqlServerTypeMappingSource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BinarySqlServerTypeMappingSource"/> class.
    /// </summary>
    /// <param name="dependencies">Type mapping source dependencies.</param>
    /// <param name="relationalDependencies">Relational type mapping source dependencies.</param>
    public BinarySqlServerTypeMappingSource(TypeMappingSourceDependencies dependencies, RelationalTypeMappingSourceDependencies relationalDependencies)
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

        var underlyingType = mappingInfo.ClrType.GenericTypeArguments[0];

        var underlyingMapping = FindMapping(underlyingType);

        if (underlyingMapping is null)
        {
            return base.FindMapping(mappingInfo);
        }

        var clrType = underlyingMapping.ClrType;

        int fixedSize
            = clrType == typeof(DateTime) ? 9
            : clrType == typeof(bool) ? 1
            : Marshal.SizeOf(clrType);
        return new SqlServerByteArrayTypeMapping(size: fixedSize, fixedLength: true);
    }
}