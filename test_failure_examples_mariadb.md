# Specific Failing Test Examples - MariaDB 11.6.2

## Category 1: RETURNING Clause Failures

### Test: TwoDatabasesMySqlTest.Can_set_connection_string_in_interceptor
**Error:**
```
Microsoft.EntityFrameworkCore.DbUpdateException : An error occurred while saving the entity changes. See the inner exception for details.
---- MySqlConnector.MySqlException : You have an error in your SQL syntax; check the manual that corresponds to your MariaDB server version for the right syntax to use near 'RETURNING 1;
```

### Test: CompositeKeyEndToEndMySqlTest.Can_use_generated_values_in_composite_key_end_to_end
**Error:**
```
Microsoft.EntityFrameworkCore.DbUpdateException : An error occurred while saving the entity changes. See the inner exception for details.
---- MySqlConnector.MySqlException : You have an error in your SQL syntax; check the manual that corresponds to your MariaDB server version for the right syntax to use near 'RETURNING 1' at line 3
```

## Category 2: LEAST Function Type Mapping Failures

### Test: NorthwindStringIncludeQueryMySqlTest.Multi_level_includes_are_applied_with_skip_take
**Error:**
```
System.InvalidOperationException : Expression 'LEAST(@p, 1)' in the SQL tree does not have a type mapping assigned.
```

### Test: NorthwindChangeTrackingQueryMySqlTest.Entity_range_does_not_revert_when_attached_dbContext
**Error:**
```
System.InvalidOperationException : Expression 'LEAST(@p, 1)' in the SQL tree does not have a type mapping assigned.
```

### Test: NorthwindMiscellaneousQueryMySqlTest.OrderBy_skip_take_take
**Error:**
```
System.InvalidOperationException : Expression 'LEAST(@p0, @p1)' in the SQL tree does not have a type mapping assigned.
```

## Category 3: SQL Generation Assertion Failures

### Test: NorthwindQueryFiltersQueryMySqlTest.Count_query
**Error:**
```
Assert.Equal() Failure: Strings differ
                                ↓ (pos 88)
Expected: ···"omers` AS `c`\nWHERE `c`.`CompanyName` LIKE"···
Actual:   ···"omers` AS `c`\nWHERE LEFT(`c`.`CompanyName"···
```
**Analysis:** The test expects LIKE clause but actual SQL uses LEFT function instead.

### Test: MySqlMigrationsSqlGeneratorTest.UpdateDataOperation_all_args_composite_multi
**Error:**
```
Assert.Equal() Failure: Strings differ
                                ↓ (pos 154)
Expected: ···" `Last Name` IS NULL;\nSELECT ROW_COUNT();"···
Actual:   ···" `Last Name` IS NULL\nRETURNING 1;\nUPDATE "···
```
**Analysis:** Test expects ROW_COUNT() pattern but code generates RETURNING clause.

## Complete List of Failing Test Names (First 50)

1. TwoDatabasesMySqlTest.Can_set_connection_string_in_interceptor(withConnectionString: True, withNullConnectionString: True)
2. TwoDatabasesMySqlTest.Can_set_connection_string_in_interceptor(withConnectionString: True, withNullConnectionString: False)
3. TwoDatabasesMySqlTest.Can_query_from_one_connection_string_and_save_changes_to_another
4. TwoDatabasesMySqlTest.Can_query_from_one_connection_and_save_changes_to_another
5. Query.NorthwindQueryFiltersQueryMySqlTest.Count_query(async: True)
6. Query.NorthwindQueryFiltersQueryMySqlTest.Count_query(async: False)
7. Query.NorthwindStringIncludeQueryMySqlTest.Multi_level_includes_are_applied_with_skip_take(async: False)
8. Query.NorthwindStringIncludeQueryMySqlTest.Multi_level_includes_are_applied_with_skip_take(async: True)
9. CompositeKeyEndToEndMySqlTest.Can_use_generated_values_in_composite_key_end_to_end
10. CompositeKeyEndToEndMySqlTest.Only_one_part_of_a_composite_key_needs_to_vary_for_uniqueness
11. CompositeKeyEndToEndMySqlTest.Can_use_two_non_generated_integers_as_composite_key_end_to_end
12. MySqlMigrationsSqlGeneratorTest.UpdateDataOperation_all_args_composite_multi
13. Query.NorthwindChangeTrackingQueryMySqlTest.Entity_range_does_not_revert_when_attached_dbContext
14. MySqlMigrationsSqlGeneratorTest.UpdateDataOperation_required_args_multiple_rows
15. MySqlMigrationsSqlGeneratorTest.UpdateDataOperation_all_args_multi
16. MySqlMigrationsSqlGeneratorTest.UpdateDataOperation_all_args
17. MySqlMigrationsSqlGeneratorTest.UpdateDataOperation_required_args_multi
18. MySqlMigrationsSqlGeneratorTest.UpdateDataOperation_required_args
19. Query.NorthwindChangeTrackingQueryMySqlTest.Entity_range_does_not_revert_when_attached_dbSet
20. MySqlMigrationsSqlGeneratorTest.UpdateDataOperation_all_args_composite
...

(9,874 total failing tests - see full output in /tmp/failed_tests_list.txt)
