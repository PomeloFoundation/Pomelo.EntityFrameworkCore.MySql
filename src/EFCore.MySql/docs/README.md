## About

_Microting.EntityFrameworkCore.MySql_ is the Entity Framework Core (EF Core) provider for [MySQL](https://www.mysql.com), [MariaDB](https://mariadb.org), [Amazon Aurora](https://aws.amazon.com/rds/aurora), [Azure Database for MySQL](https://azure.microsoft.com/en-us/services/mysql) and other MySQL-compatible databases.

It is build on top of [MySqlConnector](https://github.com/mysql-net/MySqlConnector).

## DISCLAIMER!!!
This is a fast moving version of [Microting.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql) intented to follow .net release cycle closely!
There are no intentions for this package to implement new features. Feature requests needs to be directed at the originating [Github repository](https://github.com/microting/Microting.EntityFrameworkCore.MySql)

## How to Use

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Replace with your connection string.
        var connectionString = "server=localhost;user=root;password=1234;database=ef";

        // Replace with your server version and type.
        // Use 'MariaDbServerVersion' for MariaDB.
        // Alternatively, use 'ServerVersion.AutoDetect(connectionString)'.
        // For common usages, see pull request #1233.
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));

        // Replace 'YourDbContext' with the name of your own DbContext derived class.
        services.AddDbContext<YourDbContext>(
            dbContextOptions => dbContextOptions
                .UseMySql(connectionString, serverVersion)
                // The following three options help with debugging, but should
                // be changed or removed for production.
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
        );
    }
}
```

## Key Features

* JSON support (both `Newtonsoft.Json` and `System.Text.Json`)
* Spatial support
* High performance

## Related Packages

* JSON support
  * [Microting.EntityFrameworkCore.MySql.Json.Microsoft](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql.Json.Microsoft)
  * [Microting.EntityFrameworkCore.MySql.Json.Newtonsoft](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql.Json.Newtonsoft)
* Spatial support
  * [Microting.EntityFrameworkCore.MySql.NetTopologySuite](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql.NetTopologySuite)
* Other Packages
  * [MySqlConnector](https://www.nuget.org/packages/MySqlConnector)
  * [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore)

## License

_Microting.EntityFrameworkCore.MySql_ is released as open source under the [MIT license](https://github.com/microting/Microting.EntityFrameworkCore.MySql/blob/main/LICENSE).

## Feedback

Checkout the originating [Github repository](https://github.com/microting/Microting.EntityFrameworkCore.MySql) before filing any new issues.

Bug reports and contributions are welcome at our [GitHub repository](https://github.com/microting/Microting.EntityFrameworkCore.MySql).