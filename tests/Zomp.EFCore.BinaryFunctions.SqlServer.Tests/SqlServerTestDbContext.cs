﻿namespace Zomp.EFCore.BinaryFunctions.SqlServer.Tests;

public class SqlServerTestDbContext : TestDbContext
{
    public SqlServerTestDbContext(ILoggerFactory? loggerFactory = null)
        : base(loggerFactory)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlServer(
            $@"Server=(LocalDB)\MsSqlLocalDB;Database=Zomp_EfCore_BinaryFunctions_Tests;Trusted_Connection=True",
            sqlOptions => sqlOptions.UseBinaryFunctions());
    }
}