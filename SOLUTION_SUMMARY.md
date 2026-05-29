# JSON Column Store Type Fix for EF Core 10

## Problem
EF Core 10 requires complex collections mapped with `.ToJson()` to have a provider-specific store type set. Tests were failing with:
```
The store type 'null' specified for JSON column 'Departments' is not supported by the current provider.
```

## Investigation

### Compared with SQLite and SQL Server
Both work without setting `ContainerColumnType` annotation because:
- **SQLite**: Default string mapping is "TEXT" (valid for JSON)
- **SQL Server**: Default string mapping is "nvarchar(max)" (valid for JSON)
- **MySQL**: Default string mapping is "longtext" (EF Core doesn't accept this for JSON)

### Root Causes Identified

1. **Missing annotation**: `ContainerColumnType` not set to "json" on complex types
2. **Type mapping restriction**: `MySqlTypeMappingSource` only returned JSON mapping for `string` or `MySqlJsonString` CLR types, but EF Core requests mappings with the actual collection type (e.g., `List<Department>`)

## Solution

### 1. MySqlJsonColumnConvention
**File**: `src/EFCore.MySql/Metadata/Conventions/MySqlJsonColumnConvention.cs`

- Implements `IComplexPropertyAddedConvention` and `IComplexPropertyAnnotationChangedConvention`
- Detects when `.ToJson()` is called by checking for `JsonPropertyName` annotation
- Uses `SetContainerColumnType("json")` extension method (not generic `HasAnnotation()`)
- Sets annotation on the complex type (not the complex property)

**Key insight**: Must use the specific extension method `SetContainerColumnType()` for the annotation to persist properly.

### 2. MySqlTypeMappingSource  
**File**: `src/EFCore.MySql/Storage/Internal/MySqlTypeMappingSource.cs`

Changed line 333 from:
```csharp
if (storeTypeName.Equals("json", StringComparison.OrdinalIgnoreCase) &&
    (clrType == null || clrType == typeof(string) || clrType == typeof(MySqlJsonString)))
```

To:
```csharp
if (storeTypeName.Equals("json", StringComparison.OrdinalIgnoreCase))
```

**Rationale**: JSON can serialize any object, so the type mapping should work for any CLR type when store type is explicitly "json".

## MySQL/MariaDB JSON Support

- **MySQL**: 5.7.8+ (native JSON type)
- **MariaDB**: 10.2.4+ (JSON functions, alias for LONGTEXT with validation)

Both support storing JSON in "json" column type.

## Status

**Code changes complete and committed (615e974)**

Remaining work:
- Verify convention is being triggered by EF Core convention pipeline
- Run actual failing tests with MySQL database to confirm fix
- May need to adjust convention timing if not triggering

The solution is theoretically sound based on investigation, but needs practical testing with the actual test suite.
