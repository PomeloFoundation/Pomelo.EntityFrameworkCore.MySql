# Pomelo.EntityFrameworkCore.MySql

[![Build Status](https://dev.azure.com/pomelo-efcore/Pomelo.EntityFrameworkCore.MySql/_apis/build/status/PomeloFoundation.Pomelo.EntityFrameworkCore.MySql?branchName=master)](https://dev.azure.com/pomelo-efcore/Pomelo.EntityFrameworkCore.MySql/_build/latest?definitionId=1&branchName=master)
[![NuGet](https://img.shields.io/nuget/v/Pomelo.EntityFrameworkCore.MySql.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/)
[![Pomelo.EntityFrameworkCore.MySql package in pomelo-efcore-public feed in Azure Artifacts](https://feeds.dev.azure.com/pomelo-efcore/e81f0b59-aba4-4055-8e18-e3f1a565942e/_apis/public/Packaging/Feeds/5f202e7e-2c62-4fc1-a18c-4025a32eabc8/Packages/54935cc0-f38b-4ddb-86d6-c812a8c92988/Badge)](https://dev.azure.com/pomelo-efcore/Pomelo.EntityFrameworkCore.MySql/_packaging?_a=package&feed=5f202e7e-2c62-4fc1-a18c-4025a32eabc8&package=54935cc0-f38b-4ddb-86d6-c812a8c92988&preferRelease=false)
[![Join the chat at https://gitter.im/PomeloFoundation/Home](https://badges.gitter.im/PomeloFoundation/Home.svg)](https://gitter.im/PomeloFoundation/Home?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

`Pomelo.EntityFrameworkCore.MySql` is the most popular Entity Framework Core provider for MySQL compatible databases. It supports EF Core 3.1 (and lower) and uses [MySqlConnector](https://mysqlconnector.net/) for high-performance database server communication.

## Compatibility

### Dependencies

The following versions of MySqlConnector, EF Core and .NET Standard are compatible with `Pomelo.EntityFrameworkCore.MySql`:

Release | Branch | MySqlConnector | EF Core | .NET Standard | .NET (Core) | .NET Framework
--- | --- | --- | --- | --- | --- | ---
[5.0.0-alpha.2](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/5.0.0-alpha.2) | [master](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/master) | >= 1.1.0 | 5.0.x | 2.1 | 3.0+ | N/A
[3.2.4](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/3.2.4) | [3.2-maint](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/3.2-maint) | >= 0.69.9 < 1.0.0 | 3.1.x | 2.0 | 2.0+ | 4.6.1+
[3.0.1](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/3.0.1) | [3.0-maint](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/3.0-maint) | >= 0.61.0 < 1.0.0 | 3.0.x | 2.1 | 3.0+ | N/A
[2.2.6](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/2.2.6) | [2.2-maint](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/2.2-maint) | >= 0.59.2 < 1.0.0 | 2.2.6 | 2.0 | 2.0+ | 4.6.1+

### Packages

* [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/)
* [Pomelo.EntityFrameworkCore.MySql.Json.Microsoft](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql.Json.Microsoft/)
* [Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft/)
* [Pomelo.EntityFrameworkCore.MySql.NetTopologySuite](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql.NetTopologySuite/)

### Supported Databases and Versions

`Pomelo.EntityFrameworkCore.MySql` is tested at least against the latest 2 minor versions of `MySQL` and `MariaDB`. Older versions and other server implementations _may_ be compatible (and likely are to a high degree) but are not officially supported or tested.

Currently supported versions are:

- MySQL 8.0
- MySQL 5.7
- MariaDB 10.5
- MariaDB 10.4
- MariaDB 10.3

## Schedule and Roadmap

Milestone | Status | Release Date
----------|--------|-------------
5.0.0 | In Development | TBA (see [#1088](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/1088))
5.0.0-alpha.2 | Released | 2020-11-12
3.2.4 | Released | 2020-10-13
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

### 1. Recommended Character Set

We recommend to set `utf8mb4` as your MySQL database default character set. This is already the server default in MySQL 8. The following statement will check the charset of the current database:

```sql
show variables like 'character_set_database';
```

### 2. Project Configuration

Ensure that your `.csproj` file contains the following reference:

```xml
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="5.0.0-alpha.2" />
```

### 3. Services Configuration

Add `Pomelo.EntityFrameworkCore.MySql` to the services configuration in your the `Startup.cs` file.

```csharp
using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

// Replace "YourNamespace" with the namespace of your application.
namespace YourNamespace
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Replace "YourDbContext" with the name of your own DbContext derived class.
            services.AddDbContextPool<YourDbContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(
                        // Replace with your connection string.
                        "server=localhost;user=root;password=1234;database=ef",
                        // Replace with your server version and type.
                        // For common usages, see pull request #1233.
                        new MySqlServerVersion(new Version(8, 0, 21)), // use MariaDbServerVersion for MariaDB
                        mySqlOptions => mySqlOptions
                            .CharSetBehavior(CharSetBehavior.NeverAppend))
                    // Everything from this point on is optional but helps with debugging.
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            ));
        }
    }
}
```

View our [Configuration Options Wiki Page](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/wiki/Configuration-Options) for a list of common options.

### 4. Sample Application

Check out our [Integration Tests](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/master/test/EFCore.MySql.IntegrationTests) for an example repository that includes a MVC Application.

### 5. Read the EF Core Documentation

Refer to Microsoft's [EF Core Documentation](https://docs.microsoft.com/en-us/ef/core/) for detailed instructions and examples on using EF Core.

## Scaffolding / Reverse Engineering

Use the EF Core tool to execute scaffolding commands:

```
dotnet ef dbcontext scaffold "Server=localhost;Database=ef;User=root;Password=123456;TreatTinyAsBoolean=true;" "Pomelo.EntityFrameworkCore.MySql"
```

## Contribute

One of the easiest ways to contribute is to report issues and participate in discussions. You can also contribute by submitting pull requests with code changes and supporting tests.

We are always looking for additional core contributors. If you got a couple of hours a week and know your way around EF Core and MySQL, give us a nudge.

## License

[MIT](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/blob/master/LICENSE)
