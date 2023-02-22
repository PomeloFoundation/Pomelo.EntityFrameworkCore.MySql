# Pomelo.EntityFrameworkCore.MySql

[![Build Status](https://dev.azure.com/pomelo-efcore/Pomelo.EntityFrameworkCore.MySql/_apis/build/status/PomeloFoundation.Pomelo.EntityFrameworkCore.MySql?branchName=master)](https://dev.azure.com/pomelo-efcore/Pomelo.EntityFrameworkCore.MySql/_build/latest?definitionId=1&branchName=master)
[![NuGet](https://img.shields.io/nuget/v/Pomelo.EntityFrameworkCore.MySql.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/)
[![Pomelo.EntityFrameworkCore.MySql package in pomelo-efcore-public feed in Azure Artifacts](https://feeds.dev.azure.com/pomelo-efcore/e81f0b59-aba4-4055-8e18-e3f1a565942e/_apis/public/Packaging/Feeds/5f202e7e-2c62-4fc1-a18c-4025a32eabc8/Packages/54935cc0-f38b-4ddb-86d6-c812a8c92988/Badge)](https://dev.azure.com/pomelo-efcore/Pomelo.EntityFrameworkCore.MySql/_packaging?_a=package&feed=5f202e7e-2c62-4fc1-a18c-4025a32eabc8&package=54935cc0-f38b-4ddb-86d6-c812a8c92988&preferRelease=false)
[![Join the chat at https://gitter.im/PomeloFoundation/Home](https://badges.gitter.im/PomeloFoundation/Home.svg)](https://gitter.im/PomeloFoundation/Home?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

`Pomelo.EntityFrameworkCore.MySql` is the most popular Entity Framework Core provider for MySQL compatible databases. It supports EF Core up to its latest version and uses [MySqlConnector](https://mysqlconnector.net/) for high-performance database server communication.

## Compatibility

### Dependencies

The following versions of MySqlConnector, EF Core, .NET (Core), .NET Standard and .NET Framework are compatible with published releases of `Pomelo.EntityFrameworkCore.MySql`:

Release | Branch                                                                                           | MySqlConnector     | EF Core | .NET (Core) | .NET Standard | .NET Framework
--- |--------------------------------------------------------------------------------------------------|--------------------|:---:| :---: | :---: | :---:
[7.0.0](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/7.0.0) | [master](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/master)       | >= 2.2.5           |  7.0.x  | 6.0+ | - | -
[6.0.2](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/6.0.2) | [6.0-maint](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/6.0-maint) | >= 2.1.2           |  6.0.x  | 6.0+ | - | -
[5.0.4](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/5.0.4) | [5.0-maint](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/5.0-maint) | >= 1.3.13          |  5.0.x  | 3.0+ | 2.1 | -
[3.2.7](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/3.2.7) | [3.2-maint](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/3.2-maint) | >= 0.69.10 < 1.0.0 |  3.1.x  | 2.0+ | 2.0 | 4.6.1+

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
- MariaDB 10.9
- MariaDB 10.8
- MariaDB 10.7
- MariaDB 10.6
- MariaDB 10.5
- MariaDB 10.4
- MariaDB 10.3

## Schedule and Roadmap

Milestone | Status | Release Date
----------|--------|-------------
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
        <add key="Pomelo" value="https://pkgs.dev.azure.com/pomelo-efcore/Pomelo.EntityFrameworkCore.MySql/_packaging/pomelo-efcore-public/nuget/v3/index.json" />
        <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    </packageSources>
</configuration>
```

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
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));

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
