# Failing Tests Checklist - MariaDB 11.6.2

This document provides a categorized checklist of failing test classes for tracking fixes.

## Priority 1: RETURNING Clause Issues (Critical)

- [ ] TwoDatabasesMySqlTest (4 tests)
  - [ ] Can_set_connection_string_in_interceptor (2 variants)
  - [ ] Can_query_from_one_connection_string_and_save_changes_to_another
  - [ ] Can_query_from_one_connection_and_save_changes_to_another

- [ ] CompositeKeyEndToEndMySqlTest (3 tests)
  - [ ] Can_use_generated_values_in_composite_key_end_to_end
  - [ ] Only_one_part_of_a_composite_key_needs_to_vary_for_uniqueness
  - [ ] Can_use_two_non_generated_integers_as_composite_key_end_to_end

- [ ] MySqlMigrationsSqlGeneratorTest (9 tests)
  - [ ] UpdateDataOperation_all_args_composite_multi
  - [ ] UpdateDataOperation_required_args_multiple_rows
  - [ ] UpdateDataOperation_all_args_multi
  - [ ] UpdateDataOperation_all_args
  - [ ] UpdateDataOperation_required_args_multi
  - [ ] UpdateDataOperation_required_args
  - [ ] UpdateDataOperation_all_args_composite
  - [ ] (additional update operations)

## Priority 2: Type Mapping Issues (High)

- [ ] NorthwindStringIncludeQueryMySqlTest (2 tests)
  - [ ] Multi_level_includes_are_applied_with_skip_take (async/sync)

- [ ] NorthwindChangeTrackingQueryMySqlTest (2 tests)
  - [ ] Entity_range_does_not_revert_when_attached_dbContext
  - [ ] Entity_range_does_not_revert_when_attached_dbSet

- [ ] NorthwindMiscellaneousQueryMySqlTest (multiple tests)
  - [ ] OrderBy_skip_take_take
  - [ ] (additional LEAST-related tests)

## Priority 3: Graph Updates (High - likely cascade from Priority 1)

- [ ] GraphUpdatesMySqlClientCascadeTest (985 tests)
- [ ] GraphUpdatesMySqlClientNoActionTest (820 tests)
- [ ] ProxyGraphUpdatesMySqlTest+LazyLoading (303 tests)
- [ ] ProxyGraphUpdatesMySqlTest+ChangeTrackingAndLazyLoading (303 tests)
- [ ] ProxyGraphUpdatesMySqlTest+ChangeTracking (303 tests)

**Note:** These tests likely fail due to RETURNING clause issues. Re-test after Priority 1 is fixed.

## Priority 4: Query Translation (High)

- [ ] Query.* test classes (6,681 tests across various query test suites)
  - [ ] NorthwindQueryFiltersQueryMySqlTest
  - [ ] NorthwindStringIncludeQueryMySqlTest
  - [ ] NorthwindChangeTrackingQueryMySqlTest
  - [ ] NorthwindMiscellaneousQueryMySqlTest
  - [ ] (many other Northwind query test classes)

**Note:** Review after Priority 1 and 2 fixes to identify remaining issues.

## Priority 5: Other Test Classes (Medium)

- [ ] BulkUpdates.* (94 tests)
- [ ] StoreGeneratedMySqlTest (87 tests)
- [ ] ManyToManyTrackingMySqlTest (68 tests)
- [ ] KeysWithConvertersMySqlTest (47 tests)
- [ ] MigrationsMySqlTest (21 tests)
- [ ] UpdatesMySqlTest (20 tests)
- [ ] Update.* (20 tests)
- [ ] OptimisticConcurrencyMySqlTest (18 tests)
- [ ] SaveChangesInterceptionMySqlTestBase variants (32 tests)
- [ ] FieldMappingMySqlTest (12 tests)
- [ ] Scaffolding.* (9 tests)
- [ ] TableSplittingMySqlTest (7 tests)

## Testing Strategy

1. **Fix Priority 1** (RETURNING clause support)
   - Implement MariaDB detection
   - Replace RETURNING with ROW_COUNT()/LAST_INSERT_ID()
   - Re-run full test suite

2. **Fix Priority 2** (LEAST function type mapping)
   - Add type mapping for LEAST function
   - Test affected query tests

3. **Re-evaluate Priority 3 & 4**
   - Many tests may pass after Priority 1 & 2 fixes
   - Identify remaining failures

4. **Address remaining issues**
   - Update test baselines for MariaDB-specific SQL
   - Fix any remaining query translation issues

---

## Progress Tracking

- [ ] Priority 1 fixes implemented
- [ ] Priority 2 fixes implemented
- [ ] Full test suite re-run after P1+P2
- [ ] Priority 3 re-evaluated
- [ ] Priority 4 re-evaluated
- [ ] All fixes verified
- [ ] Documentation updated

---

**Total Tests:** 30,173
**Currently Failing:** 9,874 (32.7%)
**Target:** < 1% failure rate after all fixes
