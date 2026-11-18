# Performance Analysis: .NET 9 to .NET 10 Migration

## Overview
This document analyzes potential performance issues introduced during the migration from .NET 9 (stable release v9.0.0) to .NET 10 (latest HEAD). The analysis covers 1142 commits made during this transition period.

## Executive Summary

### Critical Issues Found: 1
### Moderate Issues Found: 3
### Minor Concerns: 2

---

## Critical Performance Issues

### 1. Infinite Loop Bug in MySqlJsonTableExpression.VisitChildren()

**File**: `src/EFCore.MySql/Query/ExpressionTranslators/Internal/MySqlJsonTableExpression.cs`  
**Line**: 100  
**Severity**: CRITICAL (Infinite Loop)

#### Issue Description
The `VisitChildren()` method contains a loop increment bug that can cause an infinite loop:

```csharp
for (var j = 0; j < i; i++)  // BUG: Should be j++ not i++
{
    visitedPath[j] = Path[j];
}
```

#### Impact
- **Performance**: Infinite loop will cause application hang
- **Frequency**: Triggered when visiting JSON table expressions with path segments that need updating
- **User Impact**: Complete application freeze requiring process termination

#### Analysis
This is a typo where the loop increments `i` instead of `j`. While the loop condition checks `j < i`, the increment operation is on `i`, creating an infinite loop. This bug predates the .NET 10 migration (introduced in EF Core 8.0.0-rc.1 commit 6d38ffe4) but remains a critical performance issue.

#### Recommendation
**MUST FIX IMMEDIATELY**: Change line 100 from:
```csharp
for (var j = 0; j < i; i++)
```
to:
```csharp
for (var j = 0; j < i; j++)
```

---

## Moderate Performance Issues

### 2. String Concatenation in MySqlQueryStringFactory.PrepareCommand()

**File**: `src/EFCore.MySql/Query/Internal/MySqlQueryStringFactory.cs`  
**Lines**: 184-186  
**Severity**: MODERATE

#### Issue Description
The `PrepareCommand()` method uses inefficient string concatenation with `Substring()` to replace LEAST/GREATEST function calls:

```csharp
command.CommandText = command.CommandText.Substring(0, func.Index) +
                     func.EvaluatedValue +
                     command.CommandText.Substring(func.Index + func.FunctionCall.Length);
```

#### Impact
- **Performance**: Creates multiple intermediate string allocations
- **Frequency**: Every query execution that uses LEAST/GREATEST in LIMIT clauses
- **Memory**: Each replacement allocates 3 new strings (2 substrings + 1 concatenation)
- **Scalability**: Performance degrades with larger CommandText strings

#### Analysis
This code was introduced to handle MariaDB 11.6.2's limitation with function calls in LIMIT clauses. The string manipulation happens at query execution time, not query compilation time, making it a hot path operation.

For a CommandText of 1000 characters with a function at position 500:
- Allocates: ~500 bytes + evaluation result + ~500 bytes + concatenation overhead
- Per execution cost: ~1-2 KB allocation + CPU cycles for string operations

#### Recommendation
**FIXED**: Changed to use `StringBuilder` for string manipulation. The implementation now:
- Uses a single `StringBuilder` pre-allocated to CommandText length
- Processes replacements in a single pass from start to end
- Reduces allocations from N×3 to 1 (where N is the number of function replacements)
- Eliminates repeated string copying overhead

```csharp
if (functionsToReplace.Count > 0)
{
    var sb = new StringBuilder(command.CommandText.Length);
    var lastIndex = 0;
    
    foreach (var func in functionsToReplace.OrderBy(f => f.Index))
    {
        sb.Append(command.CommandText, lastIndex, func.Index - lastIndex);
        sb.Append(func.EvaluatedValue);
        lastIndex = func.Index + func.FunctionCall.Length;
        // ... parameter cleanup
    }
    
    if (lastIndex < command.CommandText.Length)
    {
        sb.Append(command.CommandText, lastIndex, command.CommandText.Length - lastIndex);
    }
    
    command.CommandText = sb.ToString();
}
```

**Status**: ✅ FIXED in this commit

---

### 3. Multiple String Operations in LEAST/GREATEST Evaluation

**File**: `src/EFCore.MySql/Query/ExpressionVisitors/Internal/MySqlParameterInliningExpressionVisitor.cs`  
**Lines**: 66-154  
**Severity**: MODERATE

#### Issue Description
The `VisitSqlFunction()` method processes LEAST/GREATEST functions with multiple iterations and allocations:

```csharp
// First pass: Check if all arguments can be evaluated
var canEvaluate = sqlFunctionExpression.Arguments.All(arg => 
    arg is SqlConstantExpression || 
    arg is SqlParameterExpression);

// Second pass: Visit arguments
var visitedArguments = sqlFunctionExpression.Arguments
    .Select(arg => (SqlExpression)Visit(arg))
    .ToList();

// Third pass: Extract values
foreach (var arg in visitedArguments)
{
    // Multiple type checks and conversions
    if (arg is MySqlInlinedParameterExpression inlinedParam)
    {
        value = inlinedParam.ValueExpression.Value;
    }
    else if (arg is SqlConstantExpression constant)
    {
        value = constant.Value;
    }
    
    if (value != null)
    {
        try
        {
            values.Add(Convert.ToDecimal(value));
        }
        catch { }
    }
}
```

#### Impact
- **Performance**: Three passes over arguments (evaluation check, visiting, value extraction)
- **Frequency**: Every LEAST/GREATEST function in query expressions
- **Memory**: Multiple intermediate List allocations
- **CPU**: Repeated type checks and LINQ operations

#### Analysis
This logic was added to evaluate LEAST/GREATEST functions that MariaDB 11.6.2 doesn't support in LIMIT clauses. The implementation creates multiple intermediate lists and performs redundant type checks.

Optimization opportunities:
1. Combine evaluation check with visiting in a single pass
2. Pre-allocate List with known capacity (Arguments.Count)
3. Use pattern matching more efficiently
4. Cache string comparisons (use cached StringComparison.OrdinalIgnoreCase)

#### Recommendation
**SHOULD OPTIMIZE**: Refactor to single-pass with early exit:
```csharp
var arguments = sqlFunctionExpression.Arguments;
var values = new List<decimal>(arguments.Count);
var visitedArgs = new List<SqlExpression>(arguments.Count);
var canEvaluate = true;

foreach (var arg in arguments)
{
    var visited = (SqlExpression)Visit(arg);
    visitedArgs.Add(visited);
    
    if (visited is MySqlInlinedParameterExpression inlined)
    {
        if (TryConvertToDecimal(inlined.ValueExpression.Value, out var val))
            values.Add(val);
    }
    else if (visited is SqlConstantExpression constant)
    {
        if (TryConvertToDecimal(constant.Value, out var val))
            values.Add(val);
    }
    else
    {
        canEvaluate = false;
    }
}
```

---

### 4. Missing Null Checks Leading to Defensive Coding Overhead

**Files**: Multiple visitor files  
**Commits**: 79949e2b, 6d957d07  
**Severity**: MODERATE

#### Issue Description
Multiple commits added null checks and fallback logic to prevent NullReferenceExceptions that occurred during .NET 10 migration:

```csharp
// Before (could throw NullReferenceException)
if (typeMapping is not null && !_options.ServerVersion.Supports.JsonDataTypeEmulation)
{
    return _sqlExpressionFactory.Convert(
        sqlParameterExpression,
        typeMapping.ClrType,
        typeMapping);
}

// After (defensive coding added)
if (!_options.ServerVersion.Supports.JsonDataTypeEmulation)
{
    var targetTypeMapping = typeMapping ?? sqlParameterExpression.TypeMapping;
    return _sqlExpressionFactory.Convert(
        sqlParameterExpression,
        targetTypeMapping.ClrType,
        targetTypeMapping);
}
```

#### Impact
- **Performance**: Extra null coalescing operations on hot paths
- **Frequency**: Every parameter expression visit
- **Root Cause**: Underlying type mapping issues not fully resolved

#### Analysis
The need for these null checks suggests that `FindMapping()` returns null more frequently in .NET 10 than it did in .NET 9. While the defensive coding prevents crashes, it:
1. Adds runtime overhead to check for nulls
2. May mask underlying type mapping registration issues
3. Uses fallback mappings that might not be optimal

#### Recommendation
**INVESTIGATE**: 
1. Determine why `FindMapping()` returns null more frequently in .NET 10
2. Fix root cause rather than relying on defensive coding
3. If defensive coding is necessary, profile to ensure minimal performance impact

---

## Minor Performance Concerns

### 5. CrossApply/OuterApply Visitor Pattern Changes

**File**: `src/EFCore.MySql/Query/ExpressionVisitors/Internal/MySqlCompatibilityExpressionVisitor.cs`  
**Lines**: 59-83  
**Severity**: MINOR

#### Issue Description
The visitor pattern for CrossApply/OuterApply was modified to conditionally skip compatibility checks:

```csharp
protected virtual Expression VisitCrossApply(CrossApplyExpression crossApplyExpression)
{
    // When inside DELETE/UPDATE operations with databases that don't support self-referencing subqueries,
    // skip the compatibility check to allow the query to reach the database where it will throw MySqlException (Error 1093).
    // For databases that DO support self-referencing subqueries, check support normally so proper translation/handling occurs.
    if (_insideDeleteOrUpdate && !_options.ServerVersion.Supports.DeleteWithSelfReferencingSubquery)
    {
        return crossApplyExpression.Update((TableExpressionBase)Visit(crossApplyExpression.Table));
    }
    
    return CheckSupport(crossApplyExpression, _options.ServerVersion.Supports.CrossApply);
}
```

#### Impact
- **Performance**: Adds boolean checks on every CrossApply/OuterApply visit
- **Frequency**: Proportional to query complexity (more joins = more checks)
- **Memory**: Additional boolean field `_insideDeleteOrUpdate` tracked during traversal

#### Analysis
This change introduces extra conditional logic in the visitor pattern. While the overhead is minimal (boolean checks are fast), it adds cognitive complexity and branches to the hot path.

The pattern requires maintaining state (`_insideDeleteOrUpdate`) across visitor calls, which could lead to bugs if not properly reset.

#### Recommendation
**ACCEPTABLE**: The performance impact is negligible. The pattern is correct and necessary for proper MySQL error handling. No action needed unless profiling shows measurable impact.

---

### 6. Reflection Caching (POSITIVE CHANGE)

**File**: `src/EFCore.MySql/Storage/Internal/MySqlStructuralJsonTypeMapping.cs`  
**Lines**: 25-32  
**Commit**: 230be77f  
**Severity**: MINOR (Improvement)

#### Issue Description
Good news! This commit IMPROVED performance by caching reflection lookups:

```csharp
// Cached at class level (static)
private static readonly PropertyInfo _utf8Property
    = typeof(Encoding).GetProperty(nameof(Encoding.UTF8));
private static readonly MethodInfo _getBytesMethod
    = typeof(Encoding).GetMethod(nameof(Encoding.GetBytes), new[] { typeof(string) });
private static readonly ConstructorInfo _memoryStreamConstructor
    = typeof(MemoryStream).GetConstructor(new[] { typeof(byte[]) });
```

Previously these reflection calls happened on every `CustomizeDataReaderExpression()` invocation.

#### Impact
- **Performance**: POSITIVE - eliminates repeated reflection calls
- **Frequency**: Per-row data reader operations (high frequency)
- **Improvement**: Reflection is ~1000x slower than cached member access

#### Analysis
This is a best practice for reflection-heavy code. The change caches expensive reflection lookups that would otherwise happen for every row read from the database.

#### Recommendation
**GOOD WORK**: This is a performance improvement, not an issue. Consider applying the same pattern to other areas using reflection.

---

## Additional Observations

### Positive Changes
1. **Reflection Caching**: Commit 230be77f improved performance by caching reflection lookups
2. **Code Quality**: Many null checks added improve stability (though may indicate underlying issues)

### Testing Recommendations
1. **Load Testing**: Test LEAST/GREATEST evaluation under high concurrency
2. **Memory Profiling**: Profile string allocation in PrepareCommand()
3. **Query Complexity**: Test performance with deeply nested JSON queries
4. **Benchmark**: Compare query execution times between v9.0.0 and current HEAD

### Metrics to Monitor
1. **String allocations**: Monitor GC pressure from string concatenation
2. **Query compilation time**: LEAST/GREATEST evaluation adds complexity
3. **Memory usage**: Track allocation rates for high-throughput scenarios
4. **CPU usage**: Expression visitor overhead with complex queries

---

## Summary and Recommendations

### Completed Actions
1. ✅ **FIXED CRITICAL**: MySqlJsonTableExpression infinite loop bug (line 100) - changed `i++` to `j++`
2. ✅ **FIXED MODERATE**: MySqlQueryStringFactory string concatenation - now uses StringBuilder for efficient string manipulation

### High Priority
3. **OPTIMIZE**: MySqlParameterInliningExpressionVisitor single-pass evaluation

### Medium Priority
4. **INVESTIGATE**: Root cause of FindMapping() returning null more frequently
5. **PROFILE**: Actual performance impact of new code paths under load

### Low Priority
6. **DOCUMENT**: Performance characteristics of LEAST/GREATEST evaluation
7. **BENCHMARK**: Create performance regression test suite

---

## Conclusion

The .NET 10 migration introduced several performance concerns, with one critical bug and several moderate issues that have now been addressed. The most significant performance issues related to string manipulation and potential infinite loops have been fixed in this commit.

**Overall Risk Assessment**: LOW (after fixes)  
- ~~1 critical bug (infinite loop)~~ ✅ FIXED
- ~~1 moderate performance concern (string allocation)~~ ✅ FIXED
- 1 moderate performance concern remaining (multi-pass evaluation in LEAST/GREATEST)
- Several minor concerns that may add up under high load

**Recommendation**: The critical bug and primary performance issue have been resolved. Profile real-world workloads to determine if the remaining moderate issue (multi-pass LEAST/GREATEST evaluation) causes measurable performance degradation before further optimization.

---

## Appendix: Analysis Methodology

### Commits Analyzed
- Range: v9.0.0 (stable .NET 9 release) to HEAD (current .NET 10 development)
- Total commits: 1142
- Focus: Performance-related changes, expression visitors, type mappings, query generation

### Tools Used
- Git history analysis
- Code review of changed files
- Static analysis of algorithm complexity
- Pattern matching for common performance anti-patterns

### Files Examined
- Expression visitors (MySqlCompatibilityExpressionVisitor, MySqlParameterInliningExpressionVisitor)
- Type mappings (MySqlJsonTypeMapping, MySqlStructuralJsonTypeMapping)
- Query generation (MySqlQueryStringFactory, MySqlQuerySqlGenerator)
- JSON handling (MySqlJsonTableExpression, JSON_TABLE implementation)

---

*Analysis completed: 2026-01-03*  
*Analyzer: GitHub Copilot*  
*Repository: microting/Pomelo.EntityFrameworkCore.MySql*
