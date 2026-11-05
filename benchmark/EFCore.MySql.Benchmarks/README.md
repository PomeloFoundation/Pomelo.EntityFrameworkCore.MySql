# Microting.EntityFrameworkCore.MySql Benchmarks

This project contains performance benchmarks for the Microting Entity Framework Core MySQL provider using [BenchmarkDotNet](https://benchmarkdotnet.org/).

## Purpose

These benchmarks help detect performance regressions in the provider by measuring:
- **Insert operations**: Single and batch inserts with varying complexity
- **Update operations**: Single and batch updates with different query patterns
- **Query operations**: Various retrieval patterns including filters, joins, and aggregations

## Prerequisites

- .NET 10.0 SDK or later
- MySQL 8.0+ or MariaDB 10.5+ database server
- Database credentials with permissions to create/drop databases

## Running Benchmarks Locally

### 1. Start a Database Server

**Using Docker (recommended):**

```bash
# MySQL
docker run --name mysql_benchmark -e MYSQL_ROOT_PASSWORD=Password12! -p 127.0.0.1:3306:3306 -d mysql:8.0

# MariaDB
docker run --name mariadb_benchmark -e MYSQL_ROOT_PASSWORD=Password12! -p 127.0.0.1:3306:3306 -d mariadb:11.4
```

**Or use an existing MySQL/MariaDB installation.**

### 2. Run Benchmarks

Navigate to the benchmark directory:

```bash
cd benchmark/EFCore.MySql.Benchmarks
```

Run specific benchmark categories:

```bash
# Insert benchmarks
dotnet run -c Release -- insert

# Update benchmarks
dotnet run -c Release -- update

# Query benchmarks
dotnet run -c Release -- query

# All benchmarks
dotnet run -c Release -- all
```

### 3. Configure Database Connection (Optional)

Use environment variables to configure the database connection:

```bash
# Linux/macOS
export BENCHMARK_DB_HOST=localhost
export BENCHMARK_DB_PORT=3306
export BENCHMARK_DB_NAME=pomelo_benchmark
export BENCHMARK_DB_USER=root
export BENCHMARK_DB_PASSWORD=Password12!

dotnet run -c Release -- all
```

```powershell
# Windows (PowerShell)
$env:BENCHMARK_DB_HOST="localhost"
$env:BENCHMARK_DB_PORT="3306"
$env:BENCHMARK_DB_NAME="pomelo_benchmark"
$env:BENCHMARK_DB_USER="root"
$env:BENCHMARK_DB_PASSWORD="Password12!"

dotnet run -c Release -- all
```

## Benchmark Categories

### Insert Benchmarks

Tests various insert scenarios:
- `InsertSingleSimpleEntity` - Single row insert with simple entity
- `InsertBatch10SimpleEntities` - Batch insert of 10 simple entities
- `InsertBatch100SimpleEntities` - Batch insert of 100 simple entities
- `InsertSingleComplexEntity` - Single row insert with complex entity (many columns)
- `InsertComplexEntityWithRelations` - Insert with related entities (foreign keys)
- `InsertBatch10ComplexEntities` - Batch insert of 10 complex entities

### Update Benchmarks

Tests various update scenarios:
- `UpdateSingleSimpleEntity` - Update single entity by primary key
- `UpdateBatch10SimpleEntities` - Update 10 entities in one transaction
- `UpdateSingleComplexEntity` - Update complex entity with many columns
- `UpdateComplexEntityWithQuery` - Update entity found by query
- `UpdateBatch10ComplexEntities` - Update 10 complex entities with filter

### Query Benchmarks

Tests various retrieval scenarios:
- `QuerySingleById` - Find single entity by primary key
- `QueryFirstWithFilter` - Query with WHERE clause
- `QueryTop10` - Retrieve top 10 rows
- `QueryWithFilterAndOrder` - Combined filtering and ordering
- `QueryComplexWithFilter` - Complex entity queries with filters
- `QueryComplexWithJoin` - Queries with eager loading (Include)
- `QueryComplexGroupBy` - GROUP BY with aggregations
- `QueryWithMultipleFilters` - Multiple WHERE conditions
- `QueryCount` - COUNT queries
- `QueryAny` - EXISTS-style queries
- `QueryComplexAggregation` - Complex aggregations (SUM, AVG, MAX)
- `QueryWithNavigationProperty` - Queries filtering on related entities

## Understanding Results

BenchmarkDotNet produces detailed reports including:
- **Mean**: Average execution time
- **Error**: Half of 99.9% confidence interval
- **StdDev**: Standard deviation of all measurements
- **Gen0/Gen1/Gen2**: Garbage collection statistics
- **Allocated**: Total memory allocated

Results are saved to `BenchmarkDotNet.Artifacts/` directory in the current working directory where the benchmark is executed (typically `benchmark/EFCore.MySql.Benchmarks/`).

**Note**: The artifacts directory is created relative to where you run the benchmark from. When running from the benchmark project directory, artifacts will be at `./BenchmarkDotNet.Artifacts/`. When running from the repository root with `dotnet run --project benchmark/EFCore.MySql.Benchmarks`, artifacts will be at the repository root.

## CI/CD Integration

Benchmarks can be run in GitHub Actions as part of the PR build workflow. See `.github/workflows/pr-build.yml` for configuration.

### Workflow Overview

The CI/CD pipeline includes two jobs:

1. **BuildAndTest Job** (Matrix):
   - Runs tests first to ensure correctness
   - Executes benchmarks for each database version in the matrix (31 combinations)
   - Uploads individual benchmark results as artifacts

2. **AggregateBenchmarkResults Job** (After all tests):
   - Downloads all benchmark artifacts from the matrix runs
   - Parses BenchmarkDotNet CSV reports to extract timing data
   - Updates `docs/DatabaseVersions.md` with actual performance metrics
   - Uploads the updated documentation as `benchmark-summary-documentation` artifact

### Accessing Results

After a PR build completes:
1. Go to the Actions tab
2. Find the workflow run
3. Download the `benchmark-summary-documentation` artifact
4. Extract and review `DatabaseVersions-Updated.md` for consolidated results

## Best Practices

1. **Always run in Release mode**: Debug builds have significantly different performance characteristics
2. **Close other applications**: Minimize system load during benchmarking
3. **Run multiple iterations**: BenchmarkDotNet does this automatically, but be patient
4. **Compare similar environments**: Use consistent hardware/database versions for comparisons
5. **Warm up the database**: The first run may be slower due to cold caches

## Notes

- The database specified by `BENCHMARK_DB_NAME` will be **deleted and recreated** during benchmark setup
- Benchmarks use realistic data patterns but are not comprehensive of all possible scenarios
- Performance can vary based on database version, configuration, and hardware
- These benchmarks focus on the provider's performance, not MySQL/MariaDB server performance

## Troubleshooting

### Connection Errors

If you get connection errors:
1. Verify the database is running: `docker ps` (if using Docker)
2. Check connection parameters match your setup
3. Ensure the user has CREATE/DROP database permissions

### Out of Memory

If benchmarks fail with memory errors:
- Reduce batch sizes in benchmark code
- Ensure database has sufficient resources
- Check for connection leaks

### Slow Performance

If benchmarks take too long:
- Verify running in Release mode (`-c Release`)
- Check database server isn't under heavy load
- Reduce iteration counts in `[SimpleJob]` attributes (for testing only)

## Contributing

When adding new benchmarks:
1. Follow existing patterns (inherit from `BenchmarkBase`)
2. Add meaningful scenario names
3. Include both simple and complex scenarios
4. Document what each benchmark measures
5. Test locally before committing

## Additional Resources

- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/articles/overview.html)
- [Entity Framework Core Performance](https://learn.microsoft.com/en-us/ef/core/performance/)
- [MySQL Performance Tuning](https://dev.mysql.com/doc/refman/8.0/en/optimization.html)
