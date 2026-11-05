# Complex Collection JSON Mapping Support

## Overview

Starting with EF Core 10, complex collections (collections of complex types within an entity) **must** be mapped to JSON columns. This document explains how to use this feature with the Microting MySQL provider.

## What are Complex Collections?

Complex collections are properties on an entity that contain a collection of complex types (value objects), not entities. For example:

```csharp
public class School
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    // This is a complex collection
    public List<Department> Departments { get; set; }
}

// This is a complex type (not an entity - no key)
public class Department
{
    public string Name { get; set; }
    public int Budget { get; set; }
}
```

## Database Version Requirements

JSON column support requires:
- **MySQL 5.7.8 or later** (native JSON type support)
- **MariaDB 10.2.4 or later** (JSON functions support)

## Configuration

In EF Core 10, complex collections must be explicitly mapped to JSON columns:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<School>(entity =>
    {
        entity.HasKey(e => e.Id);
        
        // Configure complex collection to use JSON column
        // The exact API is: ComplexProperty(e => e.Departments)
        // Note: The specific configuration method may vary
        entity.ComplexProperty(e => e.Departments);
    });
}
```

## Error Without Proper Configuration

If you don't configure a complex collection to use JSON, you'll get this error:

```
System.InvalidOperationException: The complex collection property 'School.Departments' 
must be mapped to a JSON column. Use 'ToJson()' to configure this complex collection 
as mapped to a JSON column.
```

## Testing

Tests that use complex collections should use the version check attribute to skip on older database versions:

```csharp
[SupportedServerVersionCondition("Json")]
public class MyComplexCollectionTests
{
    // Tests here will only run on MySQL 5.7.8+ or MariaDB 10.2.4+
}
```

## Implementation Details

The Microting provider now:
1. Allows JSON-mapped entities (previously blocked with an error)
2. Delegates validation to EF Core's base `RelationalModelValidator`
3. Supports complex collections mapped to JSON columns

## Migration from Earlier Versions

If you're upgrading from an earlier EF Core version:
1. Ensure your database version supports JSON (MySQL 5.7.8+ or MariaDB 10.2.4+)
2. Complex collections that worked with table splitting in earlier versions now require JSON mapping
3. Update your model configuration to explicitly map complex collections

## References

- [EF Core Complex Types Documentation](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-10.0/whatsnew#complex-types)
- [MySQL JSON Support](https://dev.mysql.com/doc/refman/8.0/en/json.html) (5.7.8+)
- [MariaDB JSON Functions](https://mariadb.com/kb/en/json-functions/) (10.2.4+)
