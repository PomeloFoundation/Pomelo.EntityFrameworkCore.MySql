# Microting.EntityFrameworkCore.MySql

[![Build status](https://github.com/microting/Microting.EntityFrameworkCore.MySql/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/microting/Microting.EntityFrameworkCore.MySql/actions/workflows/build.yml)
[![Stable release feed for official builds](https://img.shields.io/nuget/v/Microting.EntityFrameworkCore.MySql.svg?style=flat-square&label=Stable)](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql/)

`Microting.EntityFrameworkCore.MySql` is the most popular Entity Framework Core provider for MySQL compatible databases. It supports EF Core up to its latest version and uses [MySqlConnector](https://mysqlconnector.net/) for high-performance database server communication.

## ⚠️ Breaking Change: Fully Renamed from Pomelo to Microting

As of this release, the **assembly name** and **C# namespaces** have been changed from `Pomelo.EntityFrameworkCore.MySql` to `Microting.EntityFrameworkCore.MySql`.

**What you need to do:**

- Update any references to the provider assembly name from `Pomelo.EntityFrameworkCore.MySql` to `Microting.EntityFrameworkCore.MySql`.
- Update all `using` statements from `Pomelo.EntityFrameworkCore.MySql` to `Microting.EntityFrameworkCore.MySql`.
- If you use **EF Core scaffolding**, update your command:
  ```
  dotnet ef dbcontext scaffold "your-connection-string" "Microting.EntityFrameworkCore.MySql"
  ```

## Compatibility

### Dependencies

The following versions of MySqlConnector, EF Core, .NET (Core), .NET Standard and .NET Framework are compatible with published releases of `Microting.EntityFrameworkCore.MySql`:

Release | Branch                                                                                           | MySqlConnector     | EF Core | .NET (Core) | .NET Standard | .NET Framework
--- |--------------------------------------------------------------------------------------------------|--------------------|:-------:|:-----------:| :---: | :---:
[10.0.8](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql/10.0.8) | [master](https://github.com/microting/Microting.EntityFrameworkCore.MySql/tree/master)       | >= 2.5.0           |  10.0.x  |    10.0+     | - | -
[10.0.7](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql/10.0.7) | [master](https://github.com/microting/Microting.EntityFrameworkCore.MySql/tree/master)       | >= 2.5.0           |  10.0.x  |    10.0+     | - | -
[10.0.6](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql/10.0.6) | [master](https://github.com/microting/Microting.EntityFrameworkCore.MySql/tree/master)       | >= 2.5.0           |  10.0.x  |    10.0+     | - | -
[10.0.5](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql/10.0.5) | [master](https://github.com/microting/Microting.EntityFrameworkCore.MySql/tree/master)       | >= 2.5.0           |  10.0.x  |    10.0+     | - | -
[10.0.3](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql/10.0.3) | [master](https://github.com/microting/Microting.EntityFrameworkCore.MySql/tree/master)       | >= 2.5.0           |  10.0.x  |    10.0+     | - | -
[9.0.8](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql/9.0.8) | [master](https://github.com/microting/Microting.EntityFrameworkCore.MySql/tree/master)       | >= 2.4.0           |  9.0.x  |    9.0+     | - | -
[8.0.3](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql/8.0.3) | [8.0-maint](https://github.com/microting/Microting.EntityFrameworkCore.MySql/tree/8.0-maint) | >= 2.3.5           |  8.0.x  |    8.0+     | - | -
[7.0.0](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql/7.0.0) | [7.0-maint](https://github.com/microting/Microting.EntityFrameworkCore.MySql/tree/7.0-maint) | >= 2.2.5           |  7.0.x  |    6.0+     | - | -
[6.0.3](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql/6.0.3) | [6.0-maint](https://github.com/microting/Microting.EntityFrameworkCore.MySql/tree/6.0-maint) | >= 2.1.2           |  6.0.x  |    6.0+     | - | -
[5.0.4](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql/5.0.4) | [5.0-maint](https://github.com/microting/Microting.EntityFrameworkCore.MySql/tree/5.0-maint) | >= 1.3.13          |  5.0.x  |    3.0+     | 2.1 | -
[3.2.7](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql/3.2.7) | [3.2-maint](https://github.com/microting/Microting.EntityFrameworkCore.MySql/tree/3.2-maint) | >= 0.69.10 < 1.0.0 |  3.1.x  |    2.0+     | 2.0 | 4.6.1+

## EF Core 10 Preparation

This repository includes preparation work for EF Core 10 compatibility:

- **Conditional Compilation**: Automatic EF Core version detection with `EFCORE10_OR_GREATER`, `EFCORE9_OR_GREATER`, `EFCORE8_OR_GREATER` constants
- **Compatibility Helpers**: `EFCoreCompatibilityHelper` class for version-agnostic patterns
- **Migration Guide**: Comprehensive documentation for breaking changes ([docs/EFCore10-Migration-Guide.md](docs/EFCore10-Migration-Guide.md))
- **Test Infrastructure**: Example patterns for ExecuteUpdate API changes and migration lock interfaces

### Key EF Core 10 Breaking Changes Addressed

1. **ExecuteUpdate API**: Changed from Expression-based to Action-based setters
2. **Migration Database Locks**: New interfaces for concurrency control during migrations
3. **Query Expression Changes**: Various method signature updates

The codebase is prepared for EF Core 10 upgrade when .NET 10 becomes available, with backward compatibility maintained for current versions.

### Packages

* [Microting.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql/)
* [Microting.EntityFrameworkCore.MySql.Json.Microsoft](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql.Json.Microsoft/)
* [Microting.EntityFrameworkCore.MySql.Json.Newtonsoft](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql.Json.Newtonsoft/)
* [Microting.EntityFrameworkCore.MySql.NetTopologySuite](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql.NetTopologySuite/)

### Supported Database Servers and Versions

`Microting.EntityFrameworkCore.MySql` is tested against all actively maintained versions of `MySQL` and `MariaDB`. Older versions (e.g. MySQL 5.7) and other server implementations (e.g. Amazon Aurora) are usually compatible to a high degree as well, but are not tested as part of our CI. You can find a list of the versions, a release was tested against, within its [release](https://github.com/microting/Microting.EntityFrameworkCore.MySql/releases) notes.

Currently tested versions are:

- MySQL 8.4
- MySQL 8.0
- MariaDB 11.6
- MariaDB 11.5
- MariaDB 11.4 (LTS)
- MariaDB 11.3
- MariaDB 10.11 (LTS)
- MariaDB 10.6 (LTS)
- MariaDB 10.5 (LTS)

## Schedule and Roadmap

Milestone | Status   | Release Date
----------|----------|-------------
10.0.8 | Released | 2026-05-17
10.0.7 | Released | 2026-04-22
10.0.6 | Released | 2026-04-21
10.0.5 | Released | 2026-03-17
10.0.3 | Released | 2026-02-16
9.0.8 | Released | 2025-08-05
9.0.0 | Released | 2025-08-05
8.0.3 | Released | 2025-03-02
7.0.0 | Released | 2023-01-16
6.0.3 | Released | 2024-03-16
5.0.4 | Released | 2022-01-22
3.2.7 | Released | 2021-10-04

## Nightly Builds

To use nightly builds, add a `NuGet.config` file to your solution root with the following content and enable _prereleases_:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    </packageSources>
</configuration>
```

### Feeds

All builds are published to [NuGet.org](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql/).

All `.pdb` files use Source Link.

## Getting Started

### 1. Project Configuration

Ensure that your `.csproj` file contains the following reference:

```xml
<PackageReference Include="Microting.EntityFrameworkCore.MySql" Version="10.0.8" />
```

### 2. Services Configuration

Add `Microting.EntityFrameworkCore.MySql` to the services configuration in your `Startup.cs` file of your ASP.NET Core project:

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
        var serverVersion = new MySqlServerVersion(new Version(8, 4, 6));

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

View our [Configuration Options Wiki Page](https://github.com/microting/Microting.EntityFrameworkCore.MySql/wiki/Configuration-Options) for a list of common options.

### 3. Sample Application

Check out our [Integration Tests](https://github.com/microting/Microting.EntityFrameworkCore.MySql/tree/main/test/EFCore.MySql.IntegrationTests) for an example repository that includes an ASP.NET Core MVC Application.

There are also many complete and concise console application samples posted in the issue section (some of them can be found by searching for `Program.cs`).

### 4. Read the EF Core Documentation

Refer to Microsoft's [EF Core Documentation](https://docs.microsoft.com/en-us/ef/core/) for detailed instructions and examples on using EF Core.

## Scaffolding / Reverse Engineering

Use the [EF Core tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) to execute scaffolding commands:

```
dotnet ef dbcontext scaffold "Server=localhost;User=root;Password=1234;Database=ef" "Microting.EntityFrameworkCore.MySql"
```

## Contribute

One of the easiest ways to contribute is to report issues, participate in discussions and update the wiki docs. You can also contribute by submitting pull requests with code changes and supporting tests.

We are always looking for additional core contributors. If you got a couple of hours a week and know your way around EF Core and MySQL, give us a nudge.

## License

[MIT](https://github.com/microting/Microting.EntityFrameworkCore.MySql/blob/main/LICENSE)
