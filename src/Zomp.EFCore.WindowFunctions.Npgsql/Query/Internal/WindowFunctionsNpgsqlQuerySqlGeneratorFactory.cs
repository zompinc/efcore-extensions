﻿namespace Zomp.EFCore.WindowFunctions.Npgsql.Query.Internal;

/// <summary>
/// Factory for generating <see cref="WindowFunctionsNpgsqlQuerySqlGenerator"/>.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Multiple versions")]
public class WindowFunctionsNpgsqlQuerySqlGeneratorFactory : NpgsqlQuerySqlGeneratorFactory
{
    private readonly QuerySqlGeneratorDependencies dependencies;
    private readonly IRelationalTypeMappingSource relationalTypeMappingSource;
    private readonly INpgsqlSingletonOptions npgsqlOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowFunctionsNpgsqlQuerySqlGeneratorFactory"/> class.
    /// </summary>
    /// <param name="dependencies">Service dependencies.</param>
    /// <param name="relationalTypeMappingSource">Instance relational type mapping source.</param>
    /// <param name="npgsqlOptions">Options for Npgsql.</param>
    public WindowFunctionsNpgsqlQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies, IRelationalTypeMappingSource relationalTypeMappingSource, INpgsqlSingletonOptions npgsqlOptions)
        : base(dependencies, relationalTypeMappingSource, npgsqlOptions)
    {
        this.dependencies = dependencies;
        this.relationalTypeMappingSource = relationalTypeMappingSource;
        this.npgsqlOptions = npgsqlOptions;
    }

    /// <inheritdoc/>
    public override QuerySqlGenerator Create()
        => new WindowFunctionsNpgsqlQuerySqlGenerator(dependencies, relationalTypeMappingSource, npgsqlOptions.ReverseNullOrderingEnabled, npgsqlOptions.PostgresVersion);
}
