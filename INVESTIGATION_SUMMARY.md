# Functional Tests Investigation Summary

## Overview

This investigation analyzed all functional test failures against MariaDB 11.6.2 to identify root causes and create an action plan for fixes.

## Quick Facts

- **Investigation Date:** December 3, 2025
- **Database Tested:** MariaDB 11.6.2-MariaDB-ubu2404
- **Test Duration:** 7.33 minutes
- **Total Tests:** 30,173
- **Pass Rate:** 65.5% (19,760 passed)
- **Failure Rate:** 32.7% (9,874 failed)

## Key Findings

### Critical Issue: RETURNING Clause Incompatibility

**99% of failures** trace back to MariaDB not supporting the `RETURNING` clause used by MySQL 8.0+.

**Error Example:**
```
MySqlException: You have an error in your SQL syntax; check the manual 
that corresponds to your MariaDB server version for the right syntax 
to use near 'RETURNING 1'
```

**Solution:** Implement MariaDB-specific SQL generation using `SELECT ROW_COUNT()` or `LAST_INSERT_ID()` instead of `RETURNING`.

### High Priority: Type Mapping Issues

32+ tests fail due to `LEAST()` function not having proper type mapping:
```
InvalidOperationException: Expression 'LEAST(@p, 1)' in the SQL tree 
does not have a type mapping assigned.
```

**Solution:** Register LEAST function with appropriate type mapper in MySqlQuerySqlGenerator.

## Investigation Reports

1. **[functional_tests_failure_analysis_mariadb.md](functional_tests_failure_analysis_mariadb.md)**
   - Complete analysis with all error categories
   - Detailed statistics and breakdowns
   - Technical recommendations
   - Testing strategy

2. **[test_failure_examples_mariadb.md](test_failure_examples_mariadb.md)**
   - Specific failing test examples by category
   - Actual error messages
   - Sample test names

3. **[failing_tests_checklist.md](failing_tests_checklist.md)**
   - Prioritized checklist for tracking fixes
   - Organized by priority level
   - Progress tracking template

## Recommended Action Plan

### Phase 1: Fix RETURNING Clause (Critical)
1. Detect MariaDB server type in SQL generators
2. Replace RETURNING clause with ROW_COUNT()/LAST_INSERT_ID()
3. Update MySqlUpdateSqlGenerator and related components
4. **Expected Impact:** Should fix ~95% of failures

### Phase 2: Fix Type Mapping (High)
1. Add proper type mapping for LEAST() function
2. Update MySqlQuerySqlGenerator
3. **Expected Impact:** Should fix 32+ tests

### Phase 3: Re-evaluate & Clean Up
1. Re-run full test suite after Phase 1 & 2
2. Update test baselines for MariaDB-specific SQL
3. Address any remaining edge cases

## Test Environment Details

```
Database: MariaDB 11.6.2-MariaDB-ubu2404
Platform: Ubuntu Linux
.NET SDK: 10.0.100
Connection: localhost:3306
SQL Mode: STRICT_TRANS_TABLES,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION
Max Connections: 512
```

## Running the Tests

```bash
# Start MariaDB
docker run --name mariadb -e MYSQL_ROOT_PASSWORD=Password12! -p 127.0.0.1:3306:3306 -d mariadb:11.6.2

# Build and run tests
dotnet build -c Debug
dotnet test test/EFCore.MySql.FunctionalTests -c Debug --no-build --logger "console;verbosity=detailed"
```

**Note:** Output is ~970K lines. Grep for "Failed" to see failures.

## Next Steps

1. Review the comprehensive analysis report
2. Prioritize fixes based on impact
3. Implement RETURNING clause detection/replacement
4. Add LEAST function type mapping
5. Re-run tests and validate improvements

---

**Investigation completed successfully. All findings documented and ready for action.**
