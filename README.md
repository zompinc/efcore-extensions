# Zomp EF Core Extensions

[![Build](https://github.com/zompinc/efcore-extensions/actions/workflows/build.yml/badge.svg)](https://github.com/zompinc/efcore-extensions/actions/workflows/build.yml)
![Support .NET 6.0, .NET 8.0](https://img.shields.io/badge/dotnet%20version-net6.0,net8.0-blue)

This repository is home to two packages which extend [Entity Framework Core](https://github.com/dotnet/efcore):

- Zomp.EFCore.WindowFunctions
- Zomp.EFCore.BinaryFunctions

## Zomp.EFCore.WindowFunctions

Provides Window functions or analytics functions for providers. Currently supported for:

| Provider                                                                                         | Package                                                                                                                                                |
| ------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| [SQL Server](https://docs.microsoft.com/en-us/sql/t-sql/queries/select-over-clause-transact-sql) | [![Nuget](https://img.shields.io/nuget/v/Zomp.EFCore.WindowFunctions.SqlServer)](https://www.nuget.org/packages/Zomp.EFCore.WindowFunctions.SqlServer) |
| [PostgreSQL](https://www.postgresql.org/docs/current/tutorial-window.html)                       | [![Nuget](https://img.shields.io/nuget/v/Zomp.EFCore.WindowFunctions.Npgsql)](https://www.nuget.org/packages/Zomp.EFCore.WindowFunctions.Npgsql)       |
| [SQLite](https://www.sqlite.org/windowfunctions.html)                                            | [![Nuget](https://img.shields.io/nuget/v/Zomp.EFCore.WindowFunctions.Sqlite)](https://www.nuget.org/packages/Zomp.EFCore.WindowFunctions.Sqlite)       |

Window functions supported:

- MIN
- MAX
- SUM
- AVG
- COUNT
- ROW_NUMBER
- RANK
- DENSE_RANK
- PERCENT_RANK
- LEAD
- LAG

### Installation

To add provider-specific library use:

```sh
dotnet add package Zomp.EFCore.WindowFunctions.SqlServer
dotnet add package Zomp.EFCore.WindowFunctions.Npgsql
dotnet add package Zomp.EFCore.WindowFunctions.Sqlite
```

To add provider-agnostic library use:

```sh
dotnet add package Zomp.EFCore.WindowFunctions
```

Set up your specific provider to use Window Functions with `DbContextOptionsBuilder.UseWindowFunctions`. For example here is the SQL Server syntax:

```cs
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer(
        myConn,
        sqlOptions => sqlOptions.UseWindowFunctions());
}
```

### Basic usage

LINQ query

```cs
using var dbContext = new MyDbContext();
var query = dbContext.TestRows
.Select(r => new
{
    Max = EF.Functions.Max(
        r.Col1,
        EF.Functions.Over()
            .OrderBy(r.Col2)),
});
```

translates into the following SQL on SQL Server:

```sql
SELECT MAX([t].[Col1]) OVER(ORDER BY [t].[Col2]) AS [Max]
FROM [TestRows] AS [t]
ORDER BY [t].[Id]
```

### Advanced usage

This example shows:

- Partition clause (can be chained)
- Order by clause
  - Can me chained
  - Used in ascending or descending order
- Range or Rows clause

```cs
using var dbContext = new MyDbContext();
var query = dbContext.TestRows
.Select(r => new
{
    Max = EF.Functions.Max(
        r.Col1,
        EF.Functions.Over()
            .PartitionBy(r.Col2).ThenBy(r.Col3)
            .OrderBy(r.Col4).ThenByDescending(r.Col5)
                .Rows().FromUnbounded().ToCurrentRow()),
});
```

## Zomp.EFCore.BinaryFunctions

Provides Window functions or analytics functions for providers. Currently supported for:

| Provider   | Package                                                                                                                                                |
| ---------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| SQL Server | [![Nuget](https://img.shields.io/nuget/v/Zomp.EFCore.BinaryFunctions.SqlServer)](https://www.nuget.org/packages/Zomp.EFCore.BinaryFunctions.SqlServer) |
| PostgreSQL | [![Nuget](https://img.shields.io/nuget/v/Zomp.EFCore.BinaryFunctions.Npgsql)](https://www.nuget.org/packages/Zomp.EFCore.BinaryFunctions.Npgsql)       |
| SQLite     | [![Nuget](https://img.shields.io/nuget/v/Zomp.EFCore.BinaryFunctions.Sqlite)](https://www.nuget.org/packages/Zomp.EFCore.BinaryFunctions.Sqlite)       |

The following extension methods are available

- `DbFunctions.GetBytes` - converts an expression into binary expression
- `DbFunctions.ToValue<T>` - Converts binary expression to type T
- `DbFunctions.BinaryCast<TFrom, TTo>` - Converts one type to another by taking least significant bytes when overflow occurs.
- `DbFunctions.Concat` - concatenates two binary expressions
- `DbFunctions.Substring` - Returns part of a binary expression

### Usage

Set up your specific provider to use Binary Functions with `DbContextOptionsBuilder.UseWindowFunctions`. For example here is the SQL Server syntax:

```cs
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer(
        myConn,
        sqlOptions => sqlOptions.UseBinaryFunctions());
}
```

LINQ query

```cs
using var dbContext = new MyDbContext();
var query = dbContext.TestRows
    .Select(r => EF.Functions.GetBytes(r.Id));
```

translates into the following SQL on SQL Server:

```sql
SELECT CAST([t].[Id] AS binary(4))
FROM [TestRows] AS [t]
```

## Applications

### Last non null puzzle

One problem window functions are solving is displaying last non-null values for a given column / expressions. The problem is described in Itzik Ben-Gan's [article](https://www.itprotoday.com/sql-server/last-non-null-puzzle). Below are 2 effective approaches of solving this issue.

#### Binary approach

Solution 2 of the article above uses both binary functions and window functions. Here is how it can be combined using this library:

```cs
// Relies on Max over binary.
// Currently works with SQL Server only.
var query = dbContext.TestRows
.Select(r => new
{
    LastNonNull =
    EF.Functions.ToValue<int>(
        EF.Functions.Substring(
            EF.Functions.Max(
                EF.Functions.Concat(
                    EF.Functions.GetBytes(r.Id),
                    EF.Functions.GetBytes(r.Col1)),
                EF.Functions.Over().OrderBy(r.Id)),
            5,
            4)),
});
```

In case of limitations of combining bytes (SQLite) and window max function on binary data (PostgreSQL) it might be possible to combine columns into 8-bit integer expression(s) and perform max window function on it:

```cs
var query = dbContext.TestRows
.Select(r => new
{
    LastNonNull =
    EF.Functions.BinaryCast<long, int>(
        EF.Functions.Max(
            r.Col1.HasValue ? r.Id * (1L << 32) | r.Col1.Value & uint.MaxValue : (long?)null,
            EF.Functions.Over().OrderBy(r.Id))),
});
```

#### LAG approach

Starting with SQL Server 2022 (16.x) it is possible to use LAG with IGNORE NULLS to retrieve last non-null value. Ensure the latest cumulative update is applied due to a [bug fix](https://learn.microsoft.com/en-us/troubleshoot/sql/releases/sqlserver-2022/cumulativeupdate4#2278800).

Use the following expression:

```cs
Expression<Func<TestRow, int?>> lastNonNullExpr = r => EF.Functions.Lag(r.Col1, 0, Clauses.RespectOrIgnoreNulls.IgnoreNulls, EF.Functions.Over().OrderBy(r.Id)
```

More SQL Server related information on the LAG function here available [here](https://learn.microsoft.com/en-us/sql/t-sql/functions/lead-transact-sql?view=sql-server-ver16#-ignore-nulls--respect-nulls-).

Note: PostgreSQL and SQLite don't support RESPECT NULLS / IGNORE NULLS at this time.

## Examples

See the

- Zomp.EFCore.WindowFunctions.Testing
- Zomp.EFCore.BinaryFunctions.Testing
- Zomp.EFCore.Combined.Testing

projects for more examples.
