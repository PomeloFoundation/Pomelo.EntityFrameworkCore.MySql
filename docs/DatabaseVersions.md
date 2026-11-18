# Database Versions Tested

This document lists all MySQL and MariaDB versions tested in the CI/CD pipeline as defined in `.github/workflows/pr-build.yml`, including benchmark performance results.

## MySQL Versions

| Version | Ubuntu | Windows | Insert (ms) | Update (ms) | Query (ms) | Notes |
|---------|--------|---------|-------------|-------------|------------|-------|
| 9.5.0   | ✓      | ✗       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | Windows package not available yet |
| 9.4.0   | ✓      | ✓       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | |
| 9.3.0   | ✓      | ✓       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | |
| 9.2.0   | ✓      | ✓       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | |
| 9.1.0   | ✓      | ✓       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | |
| 9.0.1   | ✓      | ✓       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | |
| 8.4.3   | ✓      | ✓       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | |
| 8.0.40  | ✓      | ✓       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | |

## MariaDB Versions

| Version | Ubuntu | Windows | Insert (ms) | Update (ms) | Query (ms) | Notes |
|---------|--------|---------|-------------|-------------|------------|-------|
| 12.1.2  | ✓      | ✗       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | Windows package not available yet |
| 12.0.2  | ✓      | ✗       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | Windows package not available yet |
| 11.8.5  | ✓      | ✗       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | Windows package not available yet |
| 11.7.2  | ✓      | ✗       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | Different patch versions on platforms |
| 11.7.1  | ✗      | ✓       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | Different patch versions on platforms |
| 11.6.2  | ✓      | ✓       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | |
| 11.5.2  | ✓      | ✓       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | |
| 11.4.4  | ✓      | ✓       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | |
| 11.3.2  | ✓      | ✓       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | |
| 10.11.10| ✓      | ✓       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | |
| 10.6.20 | ✓      | ✓       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | |
| 10.5.27 | ✓      | ✓       | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | `TIME_PLACEHOLDER` | |

## Summary

- **MySQL**: 8 versions tested (7 on Windows, 8 on Ubuntu)
- **MariaDB**: 12 versions tested (7 on Windows, 12 on Ubuntu)
- **Total combinations**: 31 OS/version combinations in the test matrix

## Benchmark Results

The Insert, Update, and Query columns show average execution times in milliseconds for the benchmark suites:

- **Insert (ms)**: Average time for insert operations (single and batch inserts)
- **Update (ms)**: Average time for update operations (single and batch updates)
- **Query (ms)**: Average time for query operations (filters, joins, aggregations)

### Updating Benchmark Times

After benchmarks complete in CI, the `TIME_PLACEHOLDER` values should be replaced with actual performance metrics. The workflow automatically:

1. Runs benchmarks for each database version/OS combination
2. Collects BenchmarkDotNet results
3. Saves artifacts for analysis

To update this document with actual times:
- Download benchmark artifacts from GitHub Actions
- Extract mean execution times from BenchmarkDotNet reports
- Replace `TIME_PLACEHOLDER` with actual values (e.g., `12.45`)
- Note: Times are platform-specific; Windows and Linux may have different values

## SQL Modes

The following SQL modes are used for testing:

### MySQL (Current - v8.0+)
```
ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION
```

### MySQL (Legacy - v5.7 and below)
```
ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION
```

### MariaDB
```
STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION
```

Note: MariaDB currently does not use `ONLY_FULL_GROUP_BY` mode (see issue #1167).
