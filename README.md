# Pomelo.EntityFrameworkCore.MySql

[![Build Status](https://dev.azure.com/pomelo-efcore/Pomelo.EntityFrameworkCore.MySql/_apis/build/status/PomeloFoundation.Pomelo.EntityFrameworkCore.MySql?branchName=master)](https://dev.azure.com/pomelo-efcore/Pomelo.EntityFrameworkCore.MySql/_build/latest?definitionId=1&branchName=master)
[![NuGet](https://img.shields.io/nuget/v/Pomelo.EntityFrameworkCore.MySql.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/)
[![Pomelo.EntityFrameworkCore.MySql package in pomelo-efcore-public feed in Azure Artifacts](https://feeds.dev.azure.com/pomelo-efcore/e81f0b59-aba4-4055-8e18-e3f1a565942e/_apis/public/Packaging/Feeds/5f202e7e-2c62-4fc1-a18c-4025a32eabc8/Packages/54935cc0-f38b-4ddb-86d6-c812a8c92988/Badge)](https://dev.azure.com/pomelo-efcore/Pomelo.EntityFrameworkCore.MySql/_packaging?_a=package&feed=5f202e7e-2c62-4fc1-a18c-4025a32eabc8&package=54935cc0-f38b-4ddb-86d6-c812a8c92988&preferRelease=false)
[![Join the chat at https://gitter.im/PomeloFoundation/Home](https://badges.gitter.im/PomeloFoundation/Home.svg)](https://gitter.im/PomeloFoundation/Home?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

`Pomelo.EntityFrameworkCore.MySql` is the most popular Entity Framework Core provider for MySQL compatible databases. It supports EF Core up to its latest version and uses [MySqlConnector](https://mysqlconnector.net/) for high-performance database server communication.

## Compatibility

### Dependencies

The following versions of MySqlConnector, EF Core, .NET (Core), .NET Standard and .NET Framework are compatible with `Pomelo.EntityFrameworkCore.MySql`:

Release | Branch | MySqlConnector | EF Core | .NET Standard | .NET (Core) | .NET Framework
--- | --- | --- | --- | --- | --- | ---
[6.0.0-<br />preview.5](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/6.0.0-preview.5) | [master](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/master) | >= 1.3.10 | 6.0.0-<br />preview.5 | N/A | 5.0+ | N/A
[5.0.1](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/5.0.1) | [5.0-maint](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/5.0-maint) | >= 1.3.7 | 5.0.x | 2.1 | 3.0+ | N/A
[3.2.6](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/3.2.6) | [3.2-maint](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/3.2-maint) | >= 0.69.10 < 1.0.0 | 3.1.x | 2.0 | 2.0+ | 4.6.1+
[2.2.6](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/2.2.6) | [2.2-maint](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/2.2-maint) | >= 0.59.2 < 1.0.0 | 2.2.6 | 2.0 | 2.0+ | 4.6.1+

### Packages

* [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/)
* [Pomelo.EntityFrameworkCore.MySql.Json.Microsoft](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql.Json.Microsoft/)
* [Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft/)
* [Pomelo.EntityFrameworkCore.MySql.NetTopologySuite](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql.NetTopologySuite/)

### Supported Databases and Versions

`Pomelo.EntityFrameworkCore.MySql` is tested at least against the latest 2 minor versions of `MySQL` and `MariaDB`. Older versions and other server implementations (e.g. Amazon Aurora) _may_ be compatible (and likely are to a high degree) but are not officially supported or tested.

Currently supported versions are:

- MySQL 8.0
- MySQL 5.7
- MariaDB 10.5
- MariaDB 10.4
- MariaDB 10.3

## Schedule and Roadmap

Milestone | Status | Release Date
----------|--------|-------------
6.0.0 | In Development | TBA (see [#1413](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1413))
6.0.0-preview.5 | Released | 2021-06-27
6.0.0-preview.4 | Released | 2021-05-25
5.0.1 | Released | 2021-06-27
3.2.6 | Released | 2021-06-27
3.0.1 | Released | 2019-12-04
2.2.6 | Released | 2019-10-15
2.1.4 | Released | 2018-11-29

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
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="5.0.1" />
```

### 2. Services Configuration

Add `Pomelo.EntityFrameworkCore.MySql` to the services configuration in your the `Startup.cs` file.

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
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 25));

        // Replace 'YourDbContext' with the name of your own DbContext derived class.
        services.AddDbContext<YourDbContext>(
            dbContextOptions => dbContextOptions
                .UseMySql(connectionString, serverVersion)
                .EnableSensitiveDataLogging() // <-- These two calls are optional but help
                .EnableDetailedErrors()       // <-- with debugging (remove for production).
        );
    }
}
```

View our [Configuration Options Wiki Page](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/wiki/Configuration-Options) for a list of common options.

### 3. Sample Application

Check out our [Integration Tests](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/master/test/EFCore.MySql.IntegrationTests) for an example repository that includes an MVC Application.

There are also many complete and concise sample console applications posted in the issue section (some of them can be found by searching for `Program.cs`).

### 4. Read the EF Core Documentation

Refer to Microsoft's [EF Core Documentation](https://docs.microsoft.com/en-us/ef/core/) for detailed instructions and examples on using EF Core.

## Scaffolding / Reverse Engineering

Use the [EF Core tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) to execute scaffolding commands:

```
dotnet ef dbcontext scaffold "Server=localhost;Database=ef;User=root;Password=123456;TreatTinyAsBoolean=true;" "Pomelo.EntityFrameworkCore.MySql"
```

## Contribute

One of the easiest ways to contribute is to report issues, participate in discussions and update the wiki docs. You can also contribute by submitting pull requests with code changes and supporting tests.

We are always looking for additional core contributors. If you got a couple of hours a week and know your way around EF Core and MySQL, give us a nudge.

## License

[MIT](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/blob/master/LICENSE)
