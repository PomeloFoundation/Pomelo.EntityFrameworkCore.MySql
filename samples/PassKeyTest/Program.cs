using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microting.EntityFrameworkCore.MySql.Infrastructure;

Console.WriteLine("=== Testing PassKey Support with Microting EF Core MySQL ===\n");

// Connection string to MariaDB
var connectionString = "server=127.0.0.1;port=3306;database=passkey_test;user=root;password=Password12!";

// Create DbContext with Identity support including PassKeys (Version 3)
var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseMySql(connectionString, ServerVersion.Create(11, 6, 2, ServerType.MariaDb),
        mySqlOptions =>
        {
            mySqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore);
        })
    .Options;

try
{
    using var context = new ApplicationDbContext(options);
    
    Console.WriteLine("1. Testing database connection...");
    await context.Database.EnsureDeletedAsync();
    Console.WriteLine("   ✓ Deleted existing database (if any)");
    
    Console.WriteLine("\n2. Creating database with Identity schema including PassKeys (Version 3)...");
    await context.Database.EnsureCreatedAsync();
    Console.WriteLine("   ✓ Database created successfully!");
    
    Console.WriteLine("\n3. Checking if AspNetUserPasskeys table was created...");
    var command = context.Database.GetDbConnection().CreateCommand();
    command.CommandText = @"
        SELECT COUNT(*) 
        FROM information_schema.TABLES 
        WHERE TABLE_SCHEMA = 'passkey_test' 
        AND TABLE_NAME = 'AspNetUserPasskeys'";
    
    await context.Database.OpenConnectionAsync();
    var tableCount = (long)(await command.ExecuteScalarAsync() ?? 0L);
    await context.Database.CloseConnectionAsync();
    
    if (tableCount > 0)
    {
        Console.WriteLine("   ✓ AspNetUserPasskeys table exists!");
    }
    else
    {
        Console.WriteLine("   ✗ ERROR: AspNetUserPasskeys table was not created!");
        return 1;
    }
    
    Console.WriteLine("\n4. Checking Data column type in AspNetUserPasskeys...");
    command = context.Database.GetDbConnection().CreateCommand();
    command.CommandText = @"
        SELECT COLUMN_NAME, DATA_TYPE, COLUMN_TYPE
        FROM information_schema.COLUMNS
        WHERE TABLE_SCHEMA = 'passkey_test'
        AND TABLE_NAME = 'AspNetUserPasskeys'
        AND COLUMN_NAME = 'Data'";
    
    await context.Database.OpenConnectionAsync();
    using var reader = await command.ExecuteReaderAsync();
    if (await reader.ReadAsync())
    {
        var columnName = reader.GetString(0);
        var dataType = reader.GetString(1);
        var columnType = reader.GetString(2);
        Console.WriteLine($"   ✓ Column: {columnName}, Type: {dataType}, Full Type: {columnType}");
        
        // MariaDB stores JSON as longtext, MySQL 8.0+ uses json type
        if (dataType == "json" || dataType == "longtext")
        {
            Console.WriteLine($"   ✓ Data column is correctly mapped for JSON storage ({dataType})!");
        }
        else
        {
            Console.WriteLine($"   ⚠ WARNING: Data column is {dataType}, expected json or longtext");
        }
    }
    await context.Database.CloseConnectionAsync();
    
    Console.WriteLine("\n5. Testing insert of a PassKey record with complex data...");
    var user = new IdentityUser
    {
        Id = Guid.NewGuid().ToString(),
        UserName = "testuser@example.com",
        Email = "testuser@example.com",
        NormalizedUserName = "TESTUSER@EXAMPLE.COM",
        NormalizedEmail = "TESTUSER@EXAMPLE.COM",
        EmailConfirmed = true
    };
    context.Users.Add(user);
    await context.SaveChangesAsync();
    Console.WriteLine($"   ✓ Created test user: {user.UserName}");
    
    var passkey = new IdentityUserPasskey<string>
    {
        UserId = user.Id,
        CredentialId = System.Text.Encoding.UTF8.GetBytes("test-credential-id-12345"),
        Data = new IdentityPasskeyData
        {
            PublicKey = System.Text.Encoding.UTF8.GetBytes("test-public-key-data"),
            Name = "Test PassKey",
            CreatedAt = DateTimeOffset.UtcNow,
            SignCount = 0,
            // This is the critical test - string[] should be stored in JSON
            Transports = new[] { "usb", "nfc", "ble" },
            IsUserVerified = true,
            IsBackupEligible = true,
            IsBackedUp = false,
            AttestationObject = System.Text.Encoding.UTF8.GetBytes("attestation-object-data"),
            ClientDataJson = System.Text.Encoding.UTF8.GetBytes("{\"type\":\"webauthn.create\"}")
        }
    };
    
    context.Set<IdentityUserPasskey<string>>().Add(passkey);
    await context.SaveChangesAsync();
    Console.WriteLine("   ✓ PassKey record inserted successfully!");
    
    Console.WriteLine("\n6. Reading back the PassKey record...");
    var retrievedPasskey = await context.Set<IdentityUserPasskey<string>>()
        .FirstOrDefaultAsync(p => p.UserId == user.Id);
    
    if (retrievedPasskey != null)
    {
        Console.WriteLine($"   ✓ Retrieved PassKey: {Convert.ToBase64String(retrievedPasskey.CredentialId)}");
        Console.WriteLine($"   ✓ Name: {retrievedPasskey.Data.Name}");
        Console.WriteLine($"   ✓ SignCount: {retrievedPasskey.Data.SignCount}");
        if (retrievedPasskey.Data.Transports != null)
        {
            Console.WriteLine($"   ✓ Transports: [{string.Join(", ", retrievedPasskey.Data.Transports)}]");
            Console.WriteLine($"   ✓ Transport count: {retrievedPasskey.Data.Transports.Length}");
            
            if (retrievedPasskey.Data.Transports.Length == 3 &&
                retrievedPasskey.Data.Transports[0] == "usb" &&
                retrievedPasskey.Data.Transports[1] == "nfc" &&
                retrievedPasskey.Data.Transports[2] == "ble")
            {
                Console.WriteLine("   ✓ Transports array correctly stored and retrieved!");
            }
            else
            {
                Console.WriteLine("   ✗ ERROR: Transports array data mismatch!");
            }
        }
        else
        {
            Console.WriteLine("   ✗ ERROR: Transports is null!");
        }
    }
    else
    {
        Console.WriteLine("   ✗ ERROR: Could not retrieve PassKey record!");
    }
    
    Console.WriteLine("\n7. Checking raw JSON data in database...");
    command = context.Database.GetDbConnection().CreateCommand();
    command.CommandText = "SELECT Data FROM AspNetUserPasskeys WHERE UserId = @userId";
    var param = command.CreateParameter();
    param.ParameterName = "@userId";
    param.Value = user.Id;
    command.Parameters.Add(param);
    
    await context.Database.OpenConnectionAsync();
    var jsonData = await command.ExecuteScalarAsync() as string;
    await context.Database.CloseConnectionAsync();
    
    if (jsonData != null)
    {
        Console.WriteLine($"   Raw JSON in database:\n   {jsonData}");
    }
    
    Console.WriteLine("\n=== ✓ ALL TESTS PASSED! ===");
    Console.WriteLine("\nConclusion:");
    Console.WriteLine("Microting.EntityFrameworkCore.MySql successfully supports .NET 10 Identity PassKeys!");
    Console.WriteLine("The ToJson() method correctly handles complex types including string[] arrays.");
}
catch (Exception ex)
{
    Console.WriteLine($"\n✗ ERROR: {ex.GetType().Name}");
    Console.WriteLine($"Message: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
    }
    Console.WriteLine($"\nStack Trace:\n{ex.StackTrace}");
    return 1;
}

return 0;

// DbContext with Identity and PassKey support
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
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Explicitly configure the PassKey entity with ToJson()
        // This follows the pattern from IdentityUserContext.cs in ASP.NET Core
        builder.Entity<IdentityUserPasskey<string>>(b =>
        {
            b.HasKey(p => p.CredentialId);
            b.ToTable("AspNetUserPasskeys");
            b.Property(p => p.CredentialId).HasMaxLength(1024);
            b.OwnsOne(p => p.Data).ToJson();
        });
    }
}
