# Capturing SQL Baselines for NorthwindWhereQueryMySqlTest

This document explains how to capture SQL baselines for the test methods in `NorthwindWhereQueryMySqlTest.cs`.

## Prerequisites

1. MariaDB 11.6.2 or compatible MySQL server running
2. Configure `config.json` with your database connection
3. Build the project: `dotnet build -c Debug`

## Capturing a Single Test Baseline

### Step 1: Run the specific test

```bash
dotnet test test/EFCore.MySql.FunctionalTests/EFCore.MySql.FunctionalTests.csproj \
    -c Debug \
    --no-build \
    --filter "FullyQualifiedName~NorthwindWhereQueryMySqlTest.Where_simple" \
    --logger "console;verbosity=detailed"
```

### Step 2: Locate the SQL in test output

The test will fail and show output like:

```
---- New Baseline -------------------------------------------------------------------
        AssertSql(
"""
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, ...
FROM `Customers` AS `c`
WHERE `c`.`City` = 'London'
""");
```

### Step 3: Copy the SQL into the test method

Update the test method in `NorthwindWhereQueryMySqlTest.cs`:

```csharp
public override async Task Where_simple(bool async)
{
    await base.Where_simple(async);

    AssertSql(
"""
SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, ...
FROM `Customers` AS `c`
WHERE `c`.`City` = 'London'
""");
}
```

### Step 4: Verify the test passes

```bash
dotnet test test/EFCore.MySql.FunctionalTests/EFCore.MySql.FunctionalTests.csproj \
    -c Debug \
    --no-build \
    --filter "FullyQualifiedName~NorthwindWhereQueryMySqlTest.Where_simple"
```

## Batch Processing Multiple Tests

To process multiple tests at once:

```bash
# Run all Where tests and capture output
dotnet test test/EFCore.MySql.FunctionalTests/EFCore.MySql.FunctionalTests.csproj \
    -c Debug \
    --no-build \
    --filter "FullyQualifiedName~NorthwindWhereQueryMySqlTest" \
    --logger "console;verbosity=detailed" > test_output.log 2>&1

# Search for specific test SQL in the output
grep -A 20 "Where_simple" test_output.log
```

## Tests That Don't Need Baselines

Some tests pass with empty `AssertSql()` because:
- They don't generate SQL (client-side evaluation)
- The base implementation handles them correctly
- They throw expected exceptions

If a test passes without SQL, leave the `AssertSql()` empty.

## Current Status

- **Total methods**: 190
- **Tests passing with empty AssertSql()**: ~40 methods (80 test variants)
- **Tests needing SQL baselines**: ~150 methods (357 test variants)

## Using the QueryBaselineUpdater Tool

The repository includes a `QueryBaselineUpdater` tool that can help update baselines in bulk, but requires the test output in a specific format. See `tools/QueryBaselineUpdater/` for details.
