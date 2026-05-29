# Functional Issues Investigation Summary

## Overview

After fixing the two critical root causes (RETURNING clause and LEAST/GREATEST type mapping), **375 tests (40.5% of remaining 925 failures) have actual functional issues** requiring code-level fixes, not just test baseline updates.

## Issue Categories

### 1. "Sequence Contains No Elements" - 175 Tests (18.9%)

**Nature:** Query execution returns empty result sets where data is expected.

**Possible Causes:**
- Query translation differences between MySQL and MariaDB
- Missing data in test fixtures for MariaDB
- WHERE clause conditions that filter out all rows on MariaDB
- Aggregate operations (First, Single, etc.) failing on empty sets

**Investigation Needed:**
- Run subset of failing tests to capture actual error context
- Determine if issue is data-related or query translation
- Check if MariaDB-specific SQL syntax differences affect results

### 2. ComplexJSON Operations - 95 Tests (10.3%)

**Nature:** JSON operations failing on MariaDB 11.6.2.

**Known Issues:**
- JSON bulk updates
- JSON projections
- JSON set operations
- Potential differences in JSON function support between MySQL and MariaDB

**Investigation Needed:**
- Verify MariaDB 11.6.2 JSON function compatibility
- Check if JSON_TABLE, JSON_EXTRACT, etc. behave differently
- Determine if JSON type mapping needs MariaDB-specific handling

### 3. Composing Expression Errors - 47 Tests (5.1%)

**Nature:** Query translation failures when composing complex expressions.

**Likely Causes:**
- Nested query operations not translating correctly
- Complex LINQ expressions not supported by query translator
- Expression visitor issues with MariaDB-specific SQL

**Investigation Needed:**
- Identify specific expression patterns that fail
- Check if EF Core 10 changes affect expression composition
- Determine if MySQL-specific optimizations don't work on MariaDB

### 4. Other Functional Issues - 58 Tests (6.3%)

**Nature:** Miscellaneous failures not fitting other categories.

**Potential Issues:**
- LEFT JOIN not supported in certain contexts
- Type conversion differences
- Collation/character set issues
- Stored procedure compatibility

## Investigation Approach

### Phase 1: Data Analysis (2-3 hours)
1. Run sample tests from each category
2. Capture full error context and stack traces
3. Identify common patterns within each category
4. Determine root causes for each category

### Phase 2: Targeted Fixes (Variable - depends on findings)
1. **Sequence Contains No Elements**: 
   - If data issue: Update test fixtures
   - If query issue: Fix query translation
   
2. **ComplexJSON**: 
   - Check MariaDB JSON compatibility
   - Update JSON function mappings if needed
   
3. **Composing Expressions**: 
   - Fix expression visitor logic
   - Add MariaDB-specific handling
   
4. **Other Issues**: 
   - Address individually based on root cause

### Phase 3: Verification (1-2 hours)
1. Re-run affected test subsets
2. Verify fixes don't break other tests
3. Update documentation

## Time Estimates

- **Investigation**: 2-3 hours
- **Fixes**: 5-15 hours (depends on complexity)
- **Testing**: 1-2 hours
- **Total**: 8-20 hours

Compare to baseline updates: 18-45 hours for test assertions only.

## Current Status

### ✅ COMPLETED
- RETURNING clause compatibility fix
- LEAST/GREATEST type mapping fix
- 90.6% failure reduction achieved
- Comprehensive analysis and categorization

### 🔄 IN PROGRESS
- Functional issue investigation

### ⏳ PENDING
- Functional issue fixes (this document provides roadmap)
- SQL baseline updates (550 tests - can be deferred)

## Recommendations

1. **Priority**: Fix functional issues first (actual bugs)
2. **Defer**: SQL baseline updates (test assertions only)
3. **Incremental**: Address functional issues by category
4. **Verify**: Each fix with targeted test subset

## Next Steps

1. Run sample tests from "Sequence contains no elements" category
2. Capture and analyze error patterns
3. Implement targeted fixes
4. Repeat for other categories

This provides a more valuable improvement than updating 550 test baselines that don't affect production functionality.
