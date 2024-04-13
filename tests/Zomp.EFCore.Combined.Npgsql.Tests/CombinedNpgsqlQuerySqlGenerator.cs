using Microsoft.EntityFrameworkCore.Query;
#if !EF_CORE_7 && !EF_CORE_6
using Microsoft.EntityFrameworkCore.Storage;
#endif
using Zomp.EFCore.BinaryFunctions.Npgsql.Query.Internal;
using Zomp.EFCore.WindowFunctions.Query.Internal;
using Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

namespace Zomp.EFCore.Combined.Npgsql.Tests;

/// <summary>
/// Query SQL generator for Npgsql which includes binary operations and window functions.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Temporary")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Multiple versions")]
public class CombinedNpgsqlQuerySqlGenerator : BinaryNpgsqlQuerySqlGenerator
{
#if !EF_CORE_7 && !EF_CORE_6
    public CombinedNpgsqlQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies, IRelationalTypeMappingSource relationalTypeMappingSource, bool reverseNullOrderingEnabled, Version postgresVersion)
        : base(dependencies, relationalTypeMappingSource, reverseNullOrderingEnabled, postgresVersion)
    {
    }
#else
    public CombinedNpgsqlQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies, bool reverseNullOrderingEnabled, Version postgresVersion)
        : base(dependencies, reverseNullOrderingEnabled, postgresVersion)
    {
    }
#endif

    protected override Expression VisitExtension(Expression extensionExpression)
        => extensionExpression switch
        {
            WindowFunctionExpression windowFunctionExpression => this.VisitWindowFunction(windowFunctionExpression),
            _ => base.VisitExtension(extensionExpression),
        };
}
