using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using Zomp.EFCore.BinaryFunctions.Npgsql.Query.Internal;
using Zomp.EFCore.WindowFunctions.Query.Internal;
using Zomp.EFCore.WindowFunctions.Query.SqlExpressions;

namespace Zomp.EFCore.Combined.Npgsql.Tests;

/// <summary>
/// Query SQL generator for Npgsql which includes binary operations and window functions.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Temporary")]
public class CombinedNpgsqlQuerySqlGenerator : BinaryNpgsqlQuerySqlGenerator
{
    public CombinedNpgsqlQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies, bool reverseNullOrderingEnabled, Version postgresVersion)
        : base(dependencies, reverseNullOrderingEnabled, postgresVersion)
    {
    }

    protected override Expression VisitExtension(Expression extensionExpression)
        => extensionExpression switch
        {
            WindowFunctionExpression windowFunctionExpression => this.VisitWindowFunction(windowFunctionExpression),
            _ => base.VisitExtension(extensionExpression),
        };
}
