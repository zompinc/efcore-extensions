# Zomp EF Core Extensions

[![Build](https://github.com/zompinc/efcore-extensions/actions/workflows/build.yml/badge.svg)](https://github.com/zompinc/efcore-extensions/actions/workflows/build.yml)
![Support .NET 10.0](https://img.shields.io/badge/dotnet%20version-net10.0-blue)

This repository is home to two packages which extend [Entity Framework Core](https://github.com/dotnet/efcore):

- Zomp.EFCore.WindowFunctions
- Zomp.EFCore.BinaryFunctions

📺 Watch the [presentation](https://www.youtube.com/live/Z9SkvUuU9Sc) for an overview of these extensions.

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
- CUME_DIST
- NTILE
- LEAD
- LAG
- FIRST_VALUE
- LAST_VALUE (with `OrderBy` the default frame ends at the current row, so add a frame such as `.Rows().FromCurrentRow().ToUnbounded()` to get the last row of the partition)
- NTH_VALUE (not on SQL Server, which has no such function; same frame caveat as LAST_VALUE)
- Standard deviation and variance, sample and population (`StandardDeviationSample`, `StandardDeviationPopulation`, `VarianceSample`, `VariancePopulation`)

SQLite has no standard deviation or variance, so these throw there unless you opt in to an approximation computed from AVG, SUM and COUNT over the same window. It loses precision when the values are large and close together, such as timestamps:

```cs
optionsBuilder.UseSqlite(
    myConn,
    sqlOptions => sqlOptions.UseWindowFunctions(approximateStandardDeviationAndVariance: true));
```

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

### Select and Where with an index, and DistinctBy

The index overload of `Select`, which EF Core does not translate ([dotnet/efcore#24218](https://github.com/dotnet/efcore/issues/24218)), becomes `ROW_NUMBER() - 1`, ordered like the rows reaching the `Select`:

```cs
var query = dbContext.TestRows
    .OrderBy(r => r.Col1)
    .Select((r, i) => new { r.Id, Position = i + 1 });
```

```sql
SELECT [t].[Id], CAST(ROW_NUMBER() OVER(ORDER BY [t].[Col1]) - 1 AS int) + 1 AS [Position]
FROM [TestRows] AS [t]
ORDER BY [t].[Col1]
```

`Where((r, i) => ...)` is translated the same way, numbering the rows in a subquery and filtering on the number.

`DistinctBy`, which EF Core does not translate either, keeps the first row of each key the same way, with `ROW_NUMBER() OVER(PARTITION BY <key> ORDER BY ...)`. A composite key such as `new { r.A, r.B }` is partitioned by each of its members.

Rows are numbered as LINQ numbers them: after a `Where` or `Skip` before the `Select`, and before a `Where` after it. Without an `OrderBy` the rows, and so the numbers, come in no defined order. Ties in the ordering are numbered in no defined order either, so order by something unique when the numbers matter.

### TakeWhile and SkipWhile

`TakeWhile` and `SkipWhile`, which EF Core does not translate, count the rows that fail the predicate up to each row, over the ordering before them:

```sql
SUM(CASE WHEN <predicate> THEN 0 ELSE 1 END) OVER(ORDER BY ... ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
```

`TakeWhile` keeps the rows where the count is still 0 and `SkipWhile` the rest, so both stop at the first failing row as LINQ does, unlike `Where`. They need an `OrderBy`: without one there is no first failing row, and EF Core rejects the query. A comparison with NULL fails, as in C#, except when it is negated: EF Core 10 translates `!(r.Col1 <= 0)` in a condition as `NOT (...)`, which is NULL rather than true for a NULL column ([dotnet/efcore#39059](https://github.com/dotnet/efcore/issues/39059)).

### Per-group values with GroupBy and SelectMany

`GroupBy` followed by `SelectMany` over each group, which EF Core does not translate, becomes window functions partitioned by the key, so each row keeps its columns next to values of its group:

```cs
var query = dbContext.TestRows
    .GroupBy(r => r.Category)
    .SelectMany(g => g.OrderBy(r => r.Date).Select((r, i) => new
    {
        r.Id,
        Position = i + 1,
        Of = g.Count(),
        Share = (double)r.Amount / g.Sum(t => t.Amount),
    }));
```

```sql
SELECT [t].[Id], CAST(ROW_NUMBER() OVER(PARTITION BY [t].[Category] ORDER BY [t].[Date]) - 1 AS int) + 1 AS [Position],
    COUNT(*) OVER(PARTITION BY [t].[Category]) AS [Of], ...
FROM [TestRows] AS [t]
```

The index within a group, `g.Key`, and the group's `Count`, `LongCount`, `Sum`, `Min`, `Max` and `Average` are translated, with LINQ's results: an average of integers is not truncated, and a sum of nulls is 0. Any other use of the group, such as filtering it, is left for EF Core to reject.

### Correlated aggregates over the same rows

A `Count`, `LongCount`, `Sum`, `Min`, `Max` or `Average` of the rows that share a key with the current row, which EF Core runs as a correlated subquery for each row, becomes one window function partitioned by the key:

```cs
var query = dbContext.TestRows
    .Select(r => new
    {
        r.Id,
        InCategory = dbContext.TestRows.Count(t => t.Category == r.Category),
        CategoryTotal = dbContext.TestRows.Where(t => t.Category == r.Category).Sum(t => t.Amount),
    });
```

```sql
SELECT [t].[Id], COUNT(*) OVER(PARTITION BY [t].[Category]) AS [InCategory], ...
FROM [TestRows] AS [t]
```

A comparison with the current row turns it into a running aggregate or rank: counting the rows before the current one, `Count(t => t.Date < r.Date)`, becomes `RANK() OVER(ORDER BY Date) - 1`, and the rows up to and including it, `Where(t => t.Date <= r.Date).Sum(t => t.Amount)`, become `SUM(Amount) OVER(ORDER BY Date)`, whose default frame includes the rows tied with the current one. The two combine, as in a running total within each category.

The subquery must read the same rows as the outer query, with the same filters, since the window only sees those. It must be correlated by equalities of the same expression on both sides and at most one comparison of a non-nullable expression; strictly before the current row (`<` or `>`) is only translated for `Count`, as with ties it is not a window frame. Any other subquery is left as it is.

### Experimental APIs

Window functions used inside `Where`, a join or another window function are pushed down into a subquery automatically. `AsSubQuery()` forces the pushdown where nothing detects the need for it:

```cs
var query = dbContext.TestRows
.Select(r => new
{
    r.Id,
    RowNumber = EF.Functions.RowNumber(EF.Functions.Over().OrderBy(r.Id)),
})
.AsSubQuery()
.Where(r => r.Id > 2);
```

Without the call EF Core applies the filter first and the rows are numbered from 1 after filtering. It also works without any window function, for instance to stop a projected subquery from being repeated in a later `Where` ([dotnet/efcore#20291](https://github.com/dotnet/efcore/issues/20291)).

`AsSubQuery()` is marked `[Experimental]` and may be removed once EF Core no longer needs the hint. Opt in by suppressing `ZOMPEF001`:

```xml
<NoWarn>$(NoWarn);ZOMPEF001</NoWarn>
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
Expression<Func<TestRow, int?>> lastNonNullExpr = r => EF.Functions.Lag(r.Col1, 0, NullHandling.IgnoreNulls, EF.Functions.Over().OrderBy(r.Id)
```

More SQL Server related information on the LAG function here available [here](https://learn.microsoft.com/en-us/sql/t-sql/functions/lead-transact-sql?view=sql-server-ver16#-ignore-nulls--respect-nulls-).

Note: PostgreSQL and SQLite don't support RESPECT NULLS / IGNORE NULLS at this time.

## Examples

See the

- Zomp.EFCore.WindowFunctions.Testing
- Zomp.EFCore.BinaryFunctions.Testing
- Zomp.EFCore.Combined.Testing

projects for more examples.
