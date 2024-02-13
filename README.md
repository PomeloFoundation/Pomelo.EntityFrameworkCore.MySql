# Pomelo.EntityFrameworkCore.MySql

[![Build status](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/actions/workflows/build.yml)
[![Stable release feed for official builds](https://img.shields.io/nuget/v/Pomelo.EntityFrameworkCore.MySql.svg?style=flat-square&label=Stable)](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/)
[![Nightly build feed for release builds](https://img.shields.io/myget/pomelo/vpre/Pomelo.EntityFrameworkCore.MySql.svg?label=Nightly)](https://www.myget.org/feed/pomelo/package/nuget/Pomelo.EntityFrameworkCore.MySql)
[![Nightly build feed for debugging enabled builds](https://img.shields.io/myget/pomelo-debug/vpre/Pomelo.EntityFrameworkCore.MySql.svg?label=Debug)](https://www.myget.org/feed/pomelo-debug/package/nuget/Pomelo.EntityFrameworkCore.MySql)

`Pomelo.EntityFrameworkCore.MySql` is the most popular Entity Framework Core provider for MySQL compatible databases. It supports EF Core up to its latest version and uses [MySqlConnector](https://mysqlconnector.net/) for high-performance database server communication.

## Compatibility

### Dependencies

The following versions of MySqlConnector, EF Core, .NET (Core), .NET Standard and .NET Framework are compatible with published releases of `Pomelo.EntityFrameworkCore.MySql`:

Release | Branch                                                                                           | MySqlConnector     | EF Core | .NET (Core) | .NET Standard | .NET Framework
--- |--------------------------------------------------------------------------------------------------|--------------------|:-------:|:-----------:| :---: | :---:
[8.0.0-<br />beta.2](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/8.0.0-beta.2) | [master](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/master)       | >= 2.3.1           |  8.0.x  |    8.0+     | - | -
[7.0.0](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/7.0.0) | [7.0-maint](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/7.0-maint) | >= 2.2.5           |  7.0.x  |    6.0+     | - | -
[6.0.2](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/6.0.2) | [6.0-maint](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/6.0-maint) | >= 2.1.2           |  6.0.x  |    6.0+     | - | -
[5.0.4](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/5.0.4) | [5.0-maint](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/5.0-maint) | >= 1.3.13          |  5.0.x  |    3.0+     | 2.1 | -
[3.2.7](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/3.2.7) | [3.2-maint](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/3.2-maint) | >= 0.69.10 < 1.0.0 |  3.1.x  |    2.0+     | 2.0 | 4.6.1+

### Packages

* [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/)
* [Pomelo.EntityFrameworkCore.MySql.Json.Microsoft](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql.Json.Microsoft/)
* [Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft/)
* [Pomelo.EntityFrameworkCore.MySql.NetTopologySuite](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql.NetTopologySuite/)

### Supported Database Servers and Versions

`Pomelo.EntityFrameworkCore.MySql` is tested against all actively maintained versions of `MySQL` and `MariaDB`. Older versions (e.g. MySQL 5.6) and other server implementations (e.g. Amazon Aurora) are usually compatible to a high degree as well, but are not tested as part of our CI.

Officially supported versions are:

- MySQL 8.0
- MySQL 5.7
- MariaDB 11.2
- MariaDB 11.1
- MariaDB 11.0
- MariaDB 10.11 (LTS)
- MariaDB 10.10
- MariaDB 10.6 (LTS)
- MariaDB 10.5 (LTS)
- MariaDB 10.4 (LTS)

## Schedule and Roadmap

Milestone | Status   | Release Date
----------|----------|-------------
8.0.0 | Planned  | 2023-12 (see [#1746](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1746))
8.0.0-silver.1 | Planned  | 2023-12-10 (approx.)
8.0.0-beta.2 | Released | 2023-11-18
8.0.0-beta.1 | Released | 2023-09-29
7.0.0 | Released | 2023-01-16
6.0.2 | Released | 2022-07-24
5.0.4 | Released | 2022-01-22
3.2.7 | Released | 2021-10-04

## Nightly Builds

To use nightly builds from our Azure DevOps feed, add a `NuGet.config` file to your solution root with the following content and enable _prereleases_:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <add key="pomelo-nightly" value="https://pkgs.dev.azure.com/pomelo-efcore/Pomelo.EntityFrameworkCore.MySql/_packaging/pomelo-efcore-public/nuget/v3/index.json" />
        <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    </packageSources>
</configuration>
```

### Feeds

Feeds that contain optimized (`Release` configuration) builds:

* `https://pkgs.dev.azure.com/pomelo-efcore/Pomelo.EntityFrameworkCore.MySql/_packaging/pomelo-efcore-public/nuget/v3/index.json`
* `https://www.myget.org/F/pomelo/api/v3/index.json`

Feeds that contain debugging enabled unoptimized (`Debug` configuration) builds:

* `https://pkgs.dev.azure.com/pomelo-efcore/Pomelo.EntityFrameworkCore.MySql/_packaging/pomelo-efcore-debug/nuget/v3/index.json`
* `https://www.myget.org/F/pomelo-debug/api/v3/index.json`

The AZDO `nupkg` packages always contain `.pdb` files.

The MyGet `nupkg` packages only contain `.pdb` files for their debug builds. For optimized builds, the symbols are packed in a `snupkg` file and are available via the `https://www.myget.org/F/pomelo/api/v2/symbolpackage/` symbol server URL.

All `.pdb` files use Source Link.

## Getting Started

### 1. Project Configuration

Ensure that your `.csproj` file contains the following reference:

```xml
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="7.0.0" />
```

### 2. Services Configuration

Add `Pomelo.EntityFrameworkCore.MySql` to the services configuration in your the `Startup.cs` file of your ASP.NET Core project:

```c#
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
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 34));

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

View our [Configuration Options Wiki Page](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/wiki/Configuration-Options) for a list of common options.

### 3. Sample Application

Check out our [Integration Tests](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/master/test/EFCore.MySql.IntegrationTests) for an example repository that includes an ASP.NET Core MVC Application.

There are also many complete and concise console application samples posted in the issue section (some of them can be found by searching for `Program.cs`).

### 4. Read the EF Core Documentation

Refer to Microsoft's [EF Core Documentation](https://docs.microsoft.com/en-us/ef/core/) for detailed instructions and examples on using EF Core.

## Scaffolding / Reverse Engineering

Use the [EF Core tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) to execute scaffolding commands:

```
dotnet ef dbcontext scaffold "Server=localhost;User=root;Password=1234;Database=ef" "Pomelo.EntityFrameworkCore.MySql"
```

## Contribute

One of the easiest ways to contribute is to report issues, participate in discussions and update the wiki docs. You can also contribute by submitting pull requests with code changes and supporting tests.

We are always looking for additional core contributors. If you got a couple of hours a week and know your way around EF Core and MySQL, give us a nudge.

## License

[MIT](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/blob/master/LICENSE)
