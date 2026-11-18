# EF Core 10 Parameter Naming Conventions

This document describes the parameter naming conventions used in EF Core 10 and how they differ from previous versions.

## Overview

EF Core 10 introduced changes to parameter naming conventions, specifically for ExecuteUpdate (bulk update) operations. Regular DML operations (INSERT, UPDATE, DELETE) maintain the same parameter naming as EF Core 9.

## Parameter Naming Patterns

### ExecuteUpdate (Bulk Updates)

For `ExecuteUpdate` operations, EF Core 10 uses a new parameter naming convention:

#### Single Constant in SET Clause
```sql
@p='value'

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = @p
WHERE `c`.`CustomerID` LIKE 'F%'
```

#### Multiple Constants in SET Clause
```sql
@p='value1'
@p0='value2'

UPDATE `Animals` AS `a`
SET `a`.`FoundOn` = @p,
    `a`.`Name` = @p0
WHERE `a`.`Discriminator` = 'Kiwi'
```

Note: The first constant uses `@p` (no numeric suffix), subsequent constants use `@p0`, `@p1`, etc.

#### Variable/Compiled Parameters
```sql
@__value_0='Abc' (Size = 30)

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = @__value_0
WHERE `c`.`CustomerID` LIKE 'F%'
```

Note: Parameters from variables or compiled expressions use the pattern `@__variableName_N`.

#### WHERE Clause Parameters
```sql
@__p_0='4'

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = 'Updated'
WHERE `c`.`Id` = @__p_0
```

Note: Parameters in WHERE clauses use the pattern `@__p_N` (with double underscore).

#### Mixed Example
```sql
@p='Seattle' (Size = 4000)
@__value_0='Abc' (Size = 30)

UPDATE `Customers` AS `c`
SET `c`.`City` = @p,
    `c`.`ContactName` = @__value_0
WHERE `c`.`CustomerID` LIKE 'F%'
```

### Regular DML Operations (INSERT, UPDATE, DELETE)

For regular DML operations, EF Core 10 maintains the same parameter naming convention as EF Core 9:

#### INSERT Example
```sql
@p0='AC South' (Size = 4000)

INSERT INTO `AuthorsClub` (`Name`)
VALUES (@p0)
```

#### UPDATE Example (Non-Bulk)
```sql
@p0='1'
@p1='Updated' (Size = 4000)

UPDATE `Entity` SET `Name` = @p1 WHERE `Id` = @p0
```

#### DELETE Example
```sql
@p0='1'

DELETE FROM `Entity` WHERE `Id` = @p0
```

### Stored Procedure Calls

Stored procedure parameters maintain the sequential naming:

```sql
@p0='1'
@p1='Updated' (Size = 4000)
@p2='8'

CALL `EntityWithAdditionalProperty_Update`(@p0, @p1, @p2);
```

## Migration from EF Core 9 to EF Core 10

### What Changed

- **ExecuteUpdate operations**: Parameter naming changed from `@p0`, `@p1`, `@p2` to the new pattern described above
- **Regular DML operations**: No changes - still uses `@p0`, `@p1`, `@p2`
- **Stored procedures**: No changes - still uses `@p0`, `@p1`, `@p2`

### Test Updates

When updating test baselines for EF Core 10:

1. Review ExecuteUpdate test expectations
2. Ensure first constant in SET uses `@p` (no number)
3. Ensure subsequent constants use `@p0`, `@p1`, etc.
4. Verify WHERE parameters use `@__p_N` format
5. Confirm regular DML operations still use `@p0`, `@p1`, etc.

### Using QueryBaselineUpdater

To automatically update test baselines:

1. Run functional tests with MySQL
2. Collect query output
3. Use the QueryBaselineUpdater tool:
   ```bash
   dotnet run --project tools/QueryBaselineUpdater/QueryBaselineUpdater.csproj \
       <query-baseline-file> \
       <test-file-base-path>
   ```

## Test Examples

### Correct EF Core 10 Format

```csharp
// ExecuteUpdate with single constant
AssertExecuteUpdateSql(
"""
@p='Updated' (Size = 4000)

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = @p
WHERE `c`.`CustomerID` LIKE 'F%'
""");

// ExecuteUpdate with multiple constants
AssertExecuteUpdateSql(
"""
@p='0'
@p0='Kiwi' (Size = 4000)

UPDATE `Animals` AS `a`
SET `a`.`FoundOn` = @p,
    `a`.`Name` = @p0
WHERE `a`.`Discriminator` = 'Kiwi'
""");

// Regular INSERT (unchanged)
AssertSql(
"""
@p0='AC South' (Size = 4000)

INSERT INTO `AuthorsClub` (`Name`)
VALUES (@p0)
""");
```

### Incorrect Format (EF Core 9 style in ExecuteUpdate context)

```csharp
// DON'T: Using @p0 for first constant in ExecuteUpdate
AssertExecuteUpdateSql(
"""
@p0='Updated' (Size = 4000)

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = @p0
WHERE `c`.`CustomerID` LIKE 'F%'
""");

// CORRECT: Should be @p
AssertExecuteUpdateSql(
"""
@p='Updated' (Size = 4000)

UPDATE `Customers` AS `c`
SET `c`.`ContactName` = @p
WHERE `c`.`CustomerID` LIKE 'F%'
""");
```

## Current Status

As of the migration to EF Core 10, the test suite has been updated to use the correct parameter naming conventions:

- ✅ NorthwindBulkUpdatesMySqlTest.cs - Uses correct ExecuteUpdate format
- ✅ TPH/TPC/TPT InheritanceBulkUpdatesMySqlTest.cs - Uses correct ExecuteUpdate format
- ✅ NonSharedModelUpdatesMySqlTest.cs - Correctly maintains @p0 format for regular DML
- ✅ StoredProcedureUpdateMySqlTest.cs - Correctly maintains @p0 format for stored procedures

## References

- EF Core 10 Release Notes
- Microsoft.EntityFrameworkCore.Relational 10.0.0
- Microting.EntityFrameworkCore.MySql test suite
