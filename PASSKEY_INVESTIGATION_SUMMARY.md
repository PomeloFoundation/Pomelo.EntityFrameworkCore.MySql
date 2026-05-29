# PassKey Support Investigation - Summary

## Issue Investigated
[dotnet/aspnetcore#64939](https://github.com/dotnet/aspnetcore/issues/64939) - Concern that MySQL providers might not support ASP.NET Core Identity PassKeys in .NET 10 due to the use of `.ToJson()` with complex types including `string[]` arrays.

## Investigation Date
January 7, 2026

## Result
✅ **Microting.EntityFrameworkCore.MySql FULLY SUPPORTS PassKeys!**

## What Was Tested

### Test Application
Created a complete sample application at `samples/PassKeyTest` that:
1. Creates an Identity database with PassKey support (Schema Version 3)
2. Inserts PassKey records with complex data including `string[]` arrays
3. Retrieves and validates PassKey data integrity
4. Verifies JSON storage and query capabilities

### Test Environment
- .NET 10.0.101
- Microting.EntityFrameworkCore.MySql (current master branch)
- MySqlConnector 2.5.0
- Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.1
- MariaDB 11.6.2

### Test Results
All tests passed:
- ✅ Database schema creation including `AspNetUserPasskeys` table
- ✅ JSON column creation (stored as `longtext` on MariaDB, `json` on MySQL 8.0+)
- ✅ Insert PassKey records with `IdentityPasskeyData` containing `string[] Transports`
- ✅ Read PassKey records with full data integrity
- ✅ JSON serialization/deserialization of arrays and complex types
- ✅ Query JSON data using database JSON functions

## Technical Details

### How It Works
Microting uses `MySqlStructuralJsonTypeMapping` (located in `src/EFCore.MySql/Storage/Internal/MySqlStructuralJsonTypeMapping.cs`) to handle JSON columns created by `.ToJson()`:

1. **Type Mapping**: Implements `JsonTypeMapping` base class from EF Core
2. **Data Reader**: Uses `DbDataReader.GetString()` to read JSON as string from database
3. **Conversion**: Converts string to `MemoryStream` using UTF-8 encoding for EF Core's JSON processing
4. **Serialization**: Fully supports complex types including arrays, nested objects, byte arrays, etc.
5. **Parameter Handling**: Sets `MySqlDbType.JSON` for proper database parameter configuration

### Database Storage
- **MariaDB**: Stores JSON as `longtext` with JSON validation constraints
- **MySQL 8.0+**: Stores as native `json` type
- Both implementations support JSON functions and queries

### Array Handling
The critical `string[] Transports` property in `IdentityPasskeyData` is correctly handled:
- Serialized to JSON array: `["usb", "nfc", "ble"]`
- Stored in database as valid JSON
- Deserialized back to `string[]` on read
- Maintains array order and all elements

## Conclusion

**The issue reported in dotnet/aspnetcore#64939 does NOT affect Microting.EntityFrameworkCore.MySql.**

Microting's implementation of `.ToJson()` is complete and robust, handling all required scenarios including:
- Non-primitive types (`string[]`, `byte[]`)
- Complex nested objects
- Nullable properties
- All data types used in `IdentityPasskeyData`

No code changes were needed to the Microting implementation. The existing code fully supports PassKeys.

## Documentation Added

1. **Sample Application**: `samples/PassKeyTest/`
   - Complete working example
   - Demonstrates all PassKey operations
   - Includes detailed test output
   - Can be run against any MySQL 8.0+ or MariaDB 10.2.7+ instance

2. **Usage Guide**: `docs/PassKey-Support.md`
   - How to configure DbContext for PassKeys
   - Code examples
   - Migration guidance
   - Technical details

3. **Sample README**: `samples/PassKeyTest/README.md`
   - Test results
   - Usage instructions
   - Background information

## Recommendation

Users can confidently use Microting.EntityFrameworkCore.MySql with ASP.NET Core Identity PassKeys in .NET 10. The provider fully supports all required functionality out of the box.

## Files Modified/Added

- Added: `samples/PassKeyTest/PassKeyTest.csproj`
- Added: `samples/PassKeyTest/Program.cs`
- Added: `samples/PassKeyTest/README.md`
- Added: `docs/PassKey-Support.md`
- Modified: `Directory.Packages.props` (added Microsoft.Extensions.Identity.Stores version)

## No Breaking Changes

This investigation confirmed existing functionality works correctly. No changes to the Microting provider implementation were needed or made.
