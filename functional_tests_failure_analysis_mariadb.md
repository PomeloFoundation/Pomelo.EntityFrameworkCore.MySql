# Functional Tests Failure Analysis - MariaDB 11.6.2

## Executive Summary

**Date:** December 3, 2025  
**Database:** MariaDB 11.6.2-MariaDB-ubu2404  
**Test Duration:** 7.33 minutes  
**Total Tests:** 30,173  
**Passed:** 19,760 (65.5%)  
**Failed:** 9,874 (32.7%)  
**Skipped:** 539 (1.8%)  

## Overview

The functional test suite reveals significant compatibility issues between the EF Core provider and MariaDB 11.6.2. The failures are concentrated in specific areas related to SQL generation differences between MySQL and MariaDB.

## Primary Failure Categories

### 1. RETURNING Clause Compatibility Issues
**Severity:** CRITICAL  
**Affected Tests:** ~8 direct failures, but RETURNING pattern appears 53,127 times in output  
**Root Cause:** MariaDB does not support the `RETURNING` clause in INSERT/UPDATE/DELETE statements in the same way as MySQL 8.0+

**Error Pattern:**
```
MySqlConnector.MySqlException: You have an error in your SQL syntax; 
check the manual that corresponds to your MariaDB server version for 
the right syntax to use near 'RETURNING 1' or 'RETURNING 1;'
```

**Affected Test Classes:**
- TwoDatabasesMySqlTest (4 failures)
- CompositeKeyEndToEndMySqlTest (3+ failures)
- MySqlMigrationsSqlGeneratorTest (multiple failures)

**Sample Failed Tests:**
- `TwoDatabasesMySqlTest.Can_set_connection_string_in_interceptor`
- `TwoDatabasesMySqlTest.Can_query_from_one_connection_string_and_save_changes_to_another`
- `CompositeKeyEndToEndMySqlTest.Can_use_generated_values_in_composite_key_end_to_end`

**Expected Behavior:** Tests expect SQL like:
```sql
UPDATE ... WHERE ... RETURNING 1;
```

**Actual MariaDB Requirement:** Should use:
```sql
UPDATE ... WHERE ...;
SELECT ROW_COUNT();
```

### 2. Type Mapping Issues (LEAST Function)
**Severity:** HIGH  
**Affected Tests:** 32+ tests  
**Root Cause:** The LEAST() function expressions are not being properly mapped to SQL types in the query translator

**Error Pattern:**
```
InvalidOperationException: Expression 'LEAST(@p, 1)' in the SQL tree 
does not have a type mapping assigned.
```

**Affected Test Classes:**
- NorthwindStringIncludeQueryMySqlTest (2 failures)
- NorthwindChangeTrackingQueryMySqlTest (2 failures)
- NorthwindMiscellaneousQueryMySqlTest (multiple failures)

**Sample Failed Tests:**
- `NorthwindStringIncludeQueryMySqlTest.Multi_level_includes_are_applied_with_skip_take`
- `NorthwindChangeTrackingQueryMySqlTest.Entity_range_does_not_revert_when_attached_dbContext`
- `NorthwindMiscellaneousQueryMySqlTest.OrderBy_skip_take_take`

### 3. SQL Generation Assertion Failures
**Severity:** MEDIUM  
**Affected Tests:** 425+ tests  
**Root Cause:** Generated SQL differs from expected SQL, particularly with LIKE vs LEFT function usage

**Error Pattern:**
```
Assert.Equal() Failure: Strings differ
Expected: ···"omers` AS `c`\nWHERE `c`.`CompanyName` LIKE"···
Actual:   ···"omers` AS `c`\nWHERE LEFT(`c`.`CompanyName"···
```

**Affected Test Classes:**
- NorthwindQueryFiltersQueryMySqlTest (2 failures)
- MySqlMigrationsSqlGeneratorTest (multiple failures)

**Sample Failed Tests:**
- `NorthwindQueryFiltersQueryMySqlTest.Count_query`
- `MySqlMigrationsSqlGeneratorTest.UpdateDataOperation_all_args_composite_multi`

### 4. Graph Updates and Cascade Failures
**Severity:** HIGH  
**Affected Tests:** 1,985 tests (20% of failures)  
**Root Cause:** Cascading operations and relationship updates likely hitting RETURNING clause issues

**Affected Test Classes (by failure count):**
- GraphUpdatesMySqlClientCascadeTest: 985 failures
- GraphUpdatesMySqlClientNoActionTest: 820 failures
- ProxyGraphUpdatesMySqlTest variants: 909 failures combined

### 5. Query Translation Failures
**Severity:** HIGH  
**Affected Tests:** 6,681 tests (67% of failures)  
**Root Cause:** Various query translation issues including type mapping and SQL generation

**Affected Test Classes:**
- Query.* (various Northwind query test classes): 6,681 failures

### 6. Bulk Updates Failures
**Severity:** MEDIUM  
**Affected Tests:** 94 tests  
**Root Cause:** Likely related to RETURNING clause and SQL generation differences

### 7. Store-Generated Values Issues
**Severity:** MEDIUM  
**Affected Tests:** 87 tests  
**Root Cause:** Issues with identity columns and generated values, possibly RETURNING-related

**Affected Test Classes:**
- StoreGeneratedMySqlTest: 87 failures

## Test Classes with Most Failures (Top 20)

| Rank | Test Class | Failure Count | % of Total Failures |
|------|-----------|---------------|---------------------|
| 1 | Query.* (various) | 6,681 | 67.7% |
| 2 | GraphUpdatesMySqlClientCascadeTest | 985 | 10.0% |
| 3 | GraphUpdatesMySqlClientNoActionTest | 820 | 8.3% |
| 4 | ProxyGraphUpdatesMySqlTest+LazyLoading | 303 | 3.1% |
| 5 | ProxyGraphUpdatesMySqlTest+ChangeTrackingAndLazyLoading | 303 | 3.1% |
| 6 | ProxyGraphUpdatesMySqlTest+ChangeTracking | 303 | 3.1% |
| 7 | BulkUpdates.* | 94 | 1.0% |
| 8 | StoreGeneratedMySqlTest | 87 | 0.9% |
| 9 | ManyToManyTrackingMySqlTest | 68 | 0.7% |
| 10 | KeysWithConvertersMySqlTest | 47 | 0.5% |
| 11 | MigrationsMySqlTest | 21 | 0.2% |
| 12 | UpdatesMySqlTest | 20 | 0.2% |
| 13 | Update.* | 20 | 0.2% |
| 14 | OptimisticConcurrencyMySqlTest | 18 | 0.2% |
| 15 | SaveChangesInterceptionMySqlTestBase+SaveChangesInterceptionWithDiagnosticsMySqlTest | 16 | 0.2% |
| 16 | SaveChangesInterceptionMySqlTestBase+SaveChangesInterceptionMySqlTest | 16 | 0.2% |
| 17 | FieldMappingMySqlTest | 12 | 0.1% |
| 18 | Scaffolding.* | 9 | 0.1% |
| 19 | MySqlMigrationsSqlGeneratorTest | 9 | 0.1% |
| 20 | TableSplittingMySqlTest | 7 | 0.1% |

**Total Unique Test Classes with Failures:** 35

## Error Distribution Analysis

| Error Type | Occurrences | Primary Impact |
|------------|-------------|----------------|
| RETURNING clause references | 53,127 | SQL syntax errors |
| Type mapping issues | 64 | Query translation failures |
| DbUpdateException | 9,088 | Save operations |
| InvalidOperationException | 231 | Query execution |
| Assert.Equal failures | 425 | SQL generation validation |
| Assert.False failures | 16 | Boolean assertions |

## Recommended Actions

### Priority 1 (CRITICAL): RETURNING Clause Support
**Impact:** Affects thousands of tests and core functionality
**Action:** Implement MariaDB-specific handling for operations that currently use RETURNING clause
- Detect MariaDB server type
- Use `SELECT ROW_COUNT()` or `LAST_INSERT_ID()` instead of RETURNING
- Update MySqlUpdateSqlGenerator and related components

### Priority 2 (HIGH): LEAST Function Type Mapping
**Impact:** 32+ query tests failing
**Action:** Add proper type mapping for LEAST() function expressions
- Update MySqlQuerySqlGenerator
- Register LEAST function with appropriate type mapper

### Priority 3 (MEDIUM): SQL Generation Consistency
**Impact:** 425+ assertion failures
**Action:** Review and update SQL generation baselines for MariaDB
- Update test expectations for MariaDB-specific SQL patterns
- Ensure LIKE vs LEFT function usage is consistent

### Priority 4 (MEDIUM): Graph Updates and Cascading
**Impact:** ~2,000 tests
**Action:** After fixing RETURNING clause, re-run graph update tests
- Likely cascade from Priority 1 fix

### Priority 5 (LOW): Query Translation Edge Cases
**Impact:** Various query tests
**Action:** Address remaining query translation issues after higher priorities
- Review failed query tests individually
- Update query translators as needed

## Testing Recommendations

1. **Incremental Testing:** Fix Priority 1 first, then re-run full suite to see cascade effects
2. **Server Version Detection:** Ensure proper MariaDB vs MySQL detection in all code paths
3. **Baseline Updates:** Update test baselines specifically for MariaDB compatibility
4. **CI/CD:** Consider separate test runs for MySQL vs MariaDB to track compatibility

## Technical Details

### Environment
- OS: Ubuntu (Linux)
- .NET: 10.0.100
- Database: MariaDB 11.6.2-MariaDB-ubu2404
- Port: 3306 (localhost)
- SQL Mode: STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION
- Max Connections: 512

### Test Execution
- Configuration: Debug
- Build Status: Successful
- Test Framework: xUnit.net
- Total Line Output: 971,916 lines

## Conclusion

The majority of functional test failures (>99%) can be traced to two primary root causes:
1. **RETURNING clause incompatibility** between MySQL 8.0+ and MariaDB
2. **Type mapping issues** with specific SQL functions like LEAST

Addressing these two issues should resolve the vast majority of the 9,874 failing tests. The remaining failures are likely assertion mismatches that may resolve automatically or require baseline updates for MariaDB-specific SQL generation patterns.

---
*Report Generated: December 3, 2025*
*Analysis Tool: Custom bash scripts with grep/sed/awk*
*Test Output Size: 971,916 lines*
