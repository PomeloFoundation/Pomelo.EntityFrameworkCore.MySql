# PassKey Support Test for Microting.EntityFrameworkCore.MySql

## Overview

This sample project tests the support for .NET 10 ASP.NET Core Identity PassKeys with Microting.EntityFrameworkCore.MySql.

## Background

ASP.NET Core Identity in .NET 10 introduced PassKey support (WebAuthn credentials) as part of the Identity Schema Version 3. The PassKey implementation requires the `IdentityUserPasskey<TKey>` entity, which contains an `IdentityPasskeyData` property that must be stored as JSON using EF Core's `.ToJson()` method.

The `IdentityPasskeyData` class contains properties with non-primitive types, specifically `string[]` (the `Transports` property), which was reported as not working with MySQL providers in [dotnet/aspnetcore#64939](https://github.com/dotnet/aspnetcore/issues/64939).

## Test Results

✅ **All tests passed successfully!**

### What was tested:

1. **Database Creation**: The PassKey table (`AspNetUserPasskeys`) was created successfully
2. **Column Type**: The `Data` column was created as `longtext` (MariaDB's JSON implementation)
3. **Insert Operation**: PassKey records with complex data including `string[]` arrays were inserted successfully
4. **Read Operation**: PassKey records were retrieved correctly with all data intact
5. **Array Handling**: The `string[] Transports` property was correctly serialized to JSON and deserialized back
6. **JSON Validation**: The stored JSON data is valid and can be queried using MariaDB's JSON functions

### Example Output:

```
=== Testing PassKey Support with Microting EF Core MySQL ===

1. Testing database connection...
   ✓ Deleted existing database (if any)

2. Creating database with Identity schema including PassKeys (Version 3)...
   ✓ Database created successfully!

3. Checking if AspNetUserPasskeys table was created...
   ✓ AspNetUserPasskeys table exists!

4. Checking Data column type in AspNetUserPasskeys...
   ✓ Column: Data, Type: longtext, Full Type: longtext

5. Testing insert of a PassKey record with complex data...
   ✓ Created test user: testuser@example.com
   ✓ PassKey record inserted successfully!

6. Reading back the PassKey record...
   ✓ Retrieved PassKey: dGVzdC1jcmVkZW50aWFsLWlkLTEyMzQ1
   ✓ Name: Test PassKey
   ✓ SignCount: 0
   ✓ Transports: [usb, nfc, ble]
   ✓ Transport count: 3
   ✓ Transports array correctly stored and retrieved!

7. Checking raw JSON data in database...
   Raw JSON in database:
   {"AttestationObject":"...", "ClientDataJson":"...", "Transports":["usb","nfc","ble"], ...}

=== ✓ ALL TESTS PASSED! ===
```

## Conclusion

**Microting.EntityFrameworkCore.MySql successfully supports .NET 10 Identity PassKeys!**

The `ToJson()` method correctly handles complex types including `string[]` arrays. The implementation uses `MySqlStructuralJsonTypeMapping` which properly converts between MySQL's JSON storage (as longtext) and EF Core's `MemoryStream` representation.

## How to Use PassKeys with Microting

To use PassKeys with Microting.EntityFrameworkCore.MySql:

1. **Configure your DbContext** to inherit from `IdentityDbContext` with the PassKey type:

```csharp
public class ApplicationDbContext : IdentityDbContext<
    IdentityUser, 
    IdentityRole, 
    string,
    IdentityUserClaim<string>,
    IdentityUserRole<string>,
    IdentityUserLogin<string>,
    IdentityRoleClaim<string>,
    IdentityUserToken<string>,
    IdentityUserPasskey<string>>
{
    // ...
}
```

2. **Configure the PassKey entity** in `OnModelCreating`:

```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);
    
    builder.Entity<IdentityUserPasskey<string>>(b =>
    {
        b.HasKey(p => p.CredentialId);
        b.ToTable("AspNetUserPasskeys");
        b.Property(p => p.CredentialId).HasMaxLength(1024);
        b.OwnsOne(p => p.Data).ToJson(); // This is the key configuration!
    });
}
```

3. **Use Microting as your database provider**:

```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, serverVersion)
);
```

## Running This Test

1. Ensure MariaDB 11.6.2 (or MySQL 8.0+) is running:
   ```bash
   docker run --name mariadb_test -e MYSQL_ROOT_PASSWORD=Password12! -p 127.0.0.1:3306:3306 -d mariadb:11.6.2
   ```

2. Build and run the test:
   ```bash
   cd samples/PassKeyTest
   dotnet run
   ```

## Technical Details

### JSON Type Mapping

- **Storage**: MariaDB stores JSON as `longtext` with JSON validation constraints
- **Serialization**: Microting uses `MySqlStructuralJsonTypeMapping` to handle JSON columns
- **Reader/Writer**: The implementation reads strings from the database and converts them to `MemoryStream` for EF Core's JSON handling
- **Arrays**: Non-primitive types like `string[]` are correctly serialized/deserialized as JSON arrays

### Tested With

- .NET 10.0.101
- Microting.EntityFrameworkCore.MySql (current version)
- Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.1
- MariaDB 11.6.2
- MySqlConnector 2.5.0
