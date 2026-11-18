# EF Core 10 Remaining Compilation Errors

## Overview
After fixing all 352 CS0246 (missing type) errors, there are 56 remaining compilation errors related to EF Core 10 API changes in the test infrastructure. These errors are from breaking changes in the EF Core 10 test base classes and APIs.

## Error Summary

### Total Errors: 56 CS errors

| Error Code | Count | Description |
|------------|-------|-------------|
| CS0103 | 64 | Name does not exist in the current context |
| CS0117 | 10 | Type does not contain a definition |
| CS1061 | 8 | Member not found |
| CS0019 | 8 | Operator cannot be applied to operands |
| CS0618 | 6 | Member is obsolete |
| CS1662 | 4 | Cannot convert lambda expression |
| CS0029 | 4 | Cannot implicitly convert type |
| CS1501 | 2 | No overload for method takes X arguments |
| CS0411 | 2 | Type arguments cannot be inferred |
| CS0458 | 2 | Result is always null |

## Detailed Error Analysis

### 1. CS0103: Name Does Not Exist (64 instances)

**Affected Methods/Variables:**
- `AssertQuery` - Method renamed or signature changed in base class
- `LocalMethod1`, `LocalMethod2` - Test helper methods removed from base class
- `ss` - Lambda parameter naming convention changed
- `async` - Parameter name changed in method signatures

**Files Affected:**
- `Query/NonSharedPrimitiveCollectionsQueryMySqlTest.cs`
- `Query/NorthwindFunctionsQueryMySqlTest.cs`
- `Query/TPCGearsOfWarQueryMySqlTest.cs`

**Example:**
```csharp
// Error: The name 'AssertQuery' does not exist
AssertQuery(
    async,
    ss => ss.Set<TestEntity>().Where(t => t.Id > 0));
```

**Root Cause:** EF Core 10 changed the test base class API. Methods like `AssertQuery` may have been renamed, removed, or had their signatures changed.

### 2. CS0117: Member Not Found in Type (10 instances)

**Affected Methods:**
- `TrimStart_with_char_array_argument_in_predicate`
- `TrimEnd_with_char_array_argument_in_predicate`
- `Trim_with_char_array_argument_in_predicate`
- `DateTime_Compare_to_simple_zero`
- `TimeSpan_Compare_to_simple_zero`

**Files Affected:**
- `Query/NorthwindFunctionsQueryMySqlTest.cs`

**Example:**
```csharp
// Error: Base class does not contain definition for method
await base.TrimStart_with_char_array_argument_in_predicate(async);
```

**Root Cause:** These test methods were removed from the base class `NorthwindFunctionsQueryRelationalTestBase` in EF Core 10.

### 3. CS1061: Member Not Found (8 instances)

**Affected Members:**
- `Customer.SetProperty` - Extension method removed/changed
- `IRelationalCommand.CommandName` - Property removed

**Files Affected:**
- `BulkUpdates/NorthwindBulkUpdatesMySqlTest.cs`
- `TestUtilities/DebugServices/DebugRelationalCommandBuilder.cs`

**Example:**
```csharp
// Error: 'Customer' does not contain definition for 'SetProperty'
c => c.SetProperty(...)

// Error: 'IRelationalCommand' does not contain definition for 'CommandName'  
var name = command.CommandName;
```

**Root Cause:** EF Core 10 changed the ExecuteUpdate API and removed certain properties from internal interfaces.

### 4. CS0019: Operator Cannot Be Applied (8 instances)

**Type Comparison Issues:**

**Files Affected:**
- `Query/NorthwindWhereQueryMySqlTest.cs`
- `Query/NorthwindFunctionsQueryMySqlTest.cs`

**Example:**
```csharp
// Error: Operator '==' cannot be applied to operands of type 'string' and 'int'
ss => ss.Set<Customer>().Where(c => c.CustomerID == 5)
```

**Root Cause:** Type mismatches in LINQ queries that were previously allowed or handled differently.

### 5. CS0618: Obsolete API (6 instances)

**Obsolete Methods:**
- `TranslateParameterizedCollectionsToParameters()` → Use `UseParameterizedCollectionMode` instead
- `TranslateParameterizedCollectionsToConstants()` → Use `UseParameterizedCollectionMode` instead

**Files Affected:**
- `Query/NonSharedPrimitiveCollectionsQueryMySqlTest.cs`
- `TestUtilities/MySqlTestStore.cs`

**Example:**
```csharp
// Obsolete
options.UseMySql(...)
    .TranslateParameterizedCollectionsToParameters();

// Should be:
options.UseMySql(...)
    .UseParameterizedCollectionMode(ParameterizedCollectionMode.Parameters);
```

**Root Cause:** EF Core 10 deprecated old methods in favor of new enum-based configuration.

**Fix Status:** ✅ Can be easily fixed with simple replacement

### 6. CS1662/CS0029: Lambda Conversion Errors (8 instances)

**Files Affected:**
- `Query/NonSharedPrimitiveCollectionsQueryMySqlTest.cs`

**Example:**
```csharp
// Error: Cannot convert lambda expression to intended delegate type
await InitializeAsync<Context30572>(
    seed: c => 
    {
        c.Seed();
        // Some methods now return void instead of Task
    });
```

**Root Cause:** Method signatures changed, affecting return types of lambda expressions.

### 7. CS1501: No Overload for Method (2 instances)

**Affected Methods:**
- `AssertUpdate` - Signature changed

**Files Affected:**
- `BulkUpdates/NorthwindBulkUpdatesMySqlTest.cs`

**Example:**
```csharp
// Error: No overload for method 'AssertUpdate' takes 3 arguments
await AssertUpdate(async, ss => ..., rowsAffected);
```

**Root Cause:** EF Core 10 changed the `AssertUpdate` method signature.

### 8. CS0411: Type Arguments Cannot Be Inferred (2 instances)

**Affected Methods:**
- `AssertDelete<TResult>` - Type inference issue

**Files Affected:**
- `BulkUpdates/NorthwindBulkUpdatesMySqlTest.cs`

**Example:**
```csharp
// Error: Type arguments cannot be inferred
await AssertDelete(
    async,
    ss => ss.Set<OrderDetail>().Where(...));
```

**Root Cause:** EF Core 10 changed how `AssertDelete` infers generic type parameters, possibly due to changes in the method signature.

**Possible Fix:** Explicitly specify the type parameter:
```csharp
await AssertDelete<OrderDetail>(
    async,
    ss => ss.Set<OrderDetail>().Where(...));
```

### 9. CS0458: Result is Always Null (2 instances)

**Files Affected:**
- `Query/TPCGearsOfWarQueryMySqlTest.cs`

**Example:**
```csharp
// Error: The result of the expression is always 'null' of type 'AmmunitionType?'
where someCondition && ammunitionType == null
```

**Root Cause:** Compiler detecting unreachable code or logic issues in nullable enum comparisons.

## Migration Strategy

### Phase 1: Simple Fixes (Can be done immediately)
1. **Fix CS0618 (Obsolete APIs)** - Replace with new API calls
   - `TranslateParameterizedCollectionsToParameters()` → `UseParameterizedCollectionMode(ParameterizedCollectionMode.Parameters)`
   - `TranslateParameterizedCollectionsToConstants()` → `UseParameterizedCollectionMode(ParameterizedCollectionMode.Constants)`

2. **Fix CS0411 (Type Inference)** - Add explicit type parameters to AssertDelete calls

### Phase 2: Test Infrastructure Updates (Requires research)
1. **Understand EF Core 10 Test Base Class Changes**
   - Review EF Core 10 source code for test infrastructure changes
   - Identify new method signatures for `AssertQuery`, `AssertUpdate`, `AssertDelete`
   - Understand new parameter naming conventions

2. **Update Test Method Calls**
   - Replace calls to removed base class methods
   - Update lambda parameters to match new conventions
   - Fix method signatures where parameter counts changed

### Phase 3: Complex Issues (May require design decisions)
1. **CS0103 for Missing Methods** - Reimplement or remove tests for methods removed from base class
2. **CS1061 for SetProperty** - Update to new ExecuteUpdate API syntax
3. **CS0019 Type Mismatches** - Review and fix type comparison logic
4. **CS1662 Lambda Conversions** - Update lambda return types

## Recommended Next Steps

1. **Research EF Core 10 Changes**
   - Review official EF Core 10 breaking changes documentation
   - Check EF Core repository for test infrastructure migration examples
   - Look at other providers that have migrated to EF Core 10

2. **Create Test Infrastructure Compatibility Layer**
   - Consider creating extension methods to bridge API differences
   - Use conditional compilation (#if EFCORE10_OR_GREATER) where needed

3. **Incremental Migration**
   - Fix one test file at a time
   - Validate each fix by running the affected tests
   - Document patterns for consistent fixes across similar errors

4. **Get Community Input**
   - Consider reaching out to EF Core team or community
   - Check if there are migration guides for test providers

## Files Requiring Changes

| File | Error Count | Primary Issues |
|------|-------------|----------------|
| Query/NonSharedPrimitiveCollectionsQueryMySqlTest.cs | 20+ | AssertQuery, lambda conversions, obsolete APIs |
| Query/NorthwindFunctionsQueryMySqlTest.cs | 15+ | LocalMethod, ss variable, missing base methods |
| Query/TPCGearsOfWarQueryMySqlTest.cs | 10+ | ss variable, async parameter, null checks |
| BulkUpdates/NorthwindBulkUpdatesMySqlTest.cs | 6+ | AssertDelete, AssertUpdate, SetProperty |
| Query/NorthwindWhereQueryMySqlTest.cs | 4+ | Type comparisons |
| TestUtilities/MySqlTestStore.cs | 2 | Obsolete APIs |
| TestUtilities/DebugServices/DebugRelationalCommandBuilder.cs | 2 | CommandName property |
| Query/SqlQueryMySqlTest.cs | 2+ | Type issues |

## Conclusion

These 56 errors represent significant breaking changes in EF Core 10's test infrastructure. Unlike the CS0246 errors (missing types) which were resolved by adding using statements and creating missing model classes, these errors require understanding the new EF Core 10 test API patterns and updating test code accordingly.

The migration is not trivial and will require:
- Understanding new test base class APIs
- Updating method call signatures
- Replacing obsolete APIs
- Potentially rewriting some test methods

This work is separate from the initial CS0246 fix and represents the next phase of EF Core 10 migration.
