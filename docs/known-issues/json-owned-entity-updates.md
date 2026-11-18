# JSON Owned Entity Update Issue in EF Core 10

> **✅ FIXED in Pomelo**: Pomelo now uses `JSON_SET()` for partial JSON updates  
> **Upstream EF Core issue**: [dotnet/efcore#37411](https://github.com/dotnet/efcore/issues/37411)  
> **Minimum database version**: MySQL 5.7.8+ or MariaDB 10.2.3+

## Issue Description

EF Core 10 has a bug (see [dotnet/efcore#37411](https://github.com/dotnet/efcore/issues/37411)) where updating individual properties within JSON-mapped owned entities sends only the modified property value instead of the complete JSON object. Without Pomelo's fix, this would cause MySQL to reject the value with:

```
MySqlException: Invalid JSON text: "Invalid value." at position 0 in value for column 'TableName.ColumnName'.
```

## Pomelo Fix

Pomelo now works around this EF Core bug by overriding `AppendUpdateColumnValue()` to generate `JSON_SET()` SQL for partial JSON column updates. This means:

- ✅ **Direct property updates now work** — no workaround needed
- ✅ **Requires MySQL 5.7.8+ or MariaDB 10.2.3+** — which support `JSON_SET()`
- ✅ **Automatic** — Pomelo detects partial JSON updates and applies `JSON_SET()` transparently

### Example (now works)

```csharp
var user = await context.Users.FindAsync(id);
user.Data.Name = "New Name";
await context.SaveChangesAsync();  // ✅ Works — Pomelo generates JSON_SET()
```

### Generated SQL

```sql
UPDATE `Users` SET `Data` = JSON_SET(`Data`, '$.Name', @p0)
WHERE `Id` = @p1;
```

## Status and Tracking

| Item | Details |
|------|---------|
| **EF Core Bug** | [dotnet/efcore#37411](https://github.com/dotnet/efcore/issues/37411) 🔗 |
| **EF Core Status** | Open — not yet fixed upstream |
| **Pomelo Status** | ✅ Fixed — using `JSON_SET()` workaround |
| **Minimum MySQL** | 5.7.8+ |
| **Minimum MariaDB** | 10.2.3+ |

## Older Database Versions

If your database does not support `JSON_SET()`, you will get an `InvalidOperationException` with a clear message. In that case, use one of these workarounds:

### Workaround 1: Detach and Re-attach

```csharp
var user = await context.Users
    .AsNoTracking()
    .FirstAsync(u => u.Id == id);
user.Data.Name = "New Name";
context.Users.Update(user);
await context.SaveChangesAsync();
```

### Workaround 2: Replace the Entire Object

```csharp
var user = await context.Users.FindAsync(id);
user.Data = new PasskeyData
{
    Name = "New Name",
    Transports = user.Data.Transports,
};
await context.SaveChangesAsync();
```

## Related Issues

- ASP.NET Core Identity PassKeys are affected when renaming passkeys
- Any scenario involving updates to properties within JSON-mapped owned entities
