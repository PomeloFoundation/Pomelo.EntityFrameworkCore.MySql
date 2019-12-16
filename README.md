# Pomelo.EntityFrameworkCore.MySql

[![Build Status](https://dev.azure.com/pomelo-efcore/Pomelo.EntityFrameworkCore.MySql/_apis/build/status/PomeloFoundation.Pomelo.EntityFrameworkCore.MySql?branchName=master)](https://dev.azure.com/pomelo-efcore/Pomelo.EntityFrameworkCore.MySql/_build/latest?definitionId=1&branchName=master)
[![NuGet](https://img.shields.io/nuget/v/Pomelo.EntityFrameworkCore.MySql.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/)
[![Pomelo.EntityFrameworkCore.MySql package in pomelo-efcore-public feed in Azure Artifacts](https://feeds.dev.azure.com/pomelo-efcore/e81f0b59-aba4-4055-8e18-e3f1a565942e/_apis/public/Packaging/Feeds/5f202e7e-2c62-4fc1-a18c-4025a32eabc8/Packages/54935cc0-f38b-4ddb-86d6-c812a8c92988/Badge)](https://dev.azure.com/pomelo-efcore/Pomelo.EntityFrameworkCore.MySql/_packaging?_a=package&feed=5f202e7e-2c62-4fc1-a18c-4025a32eabc8&package=54935cc0-f38b-4ddb-86d6-c812a8c92988&preferRelease=false)
[![Join the chat at https://gitter.im/PomeloFoundation/Home](https://badges.gitter.im/PomeloFoundation/Home.svg)](https://gitter.im/PomeloFoundation/Home?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

`Pomelo.EntityFrameworkCore.MySql` is an Entity Framework Core provider built on top of [MySqlConnector](https://github.com/mysql-net/MySqlConnector) that enables the use of the Entity Framework Core ORM with MySQL.

## Compatibility

### EF Core and .NET Standard

The following versions of EF Core and .NET Standard are compatible with `Pomelo.EntityFrameworkCore.MySql`:

Pomelo.EntityFrameworkCore.MySql | EF Core | .NET Standard | .NET Core | .NET Framework
-- | -- | -- | -- | --
[3.1.0](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/3.1.0) | 3.1.0 | 2.0 | 2.0+ | 4.7.2+
[3.0.1](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/3.0.1) | 3.0.1 | 2.1 | 3.0+ | N/A
[2.2.6](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/2.2.6) | 2.2.6 | 2.0 | 2.0+ | 4.7.2+

### Supported DBMS and Versions

`Pomelo.EntityFrameworkCore.MySql` is tested against the latest 2 minor versions of `MySQL` and `MariaDB`. Older versions _may_ be compatible but are not officially supported or tested.  Currently supported versions are:

- MySQL 8.0
- MySQL 5.7
- MariaDB 10.4
- MariaDB 10.3

## Schedule and Roadmap

Milestone | Status | Release Date
----------|--------|-------------
3.1.0 | Released | 2019-12-16
3.1.0-rc1 | Released | 2019-12-06
3.0.1 | Released | 2019-12-04
3.0.0 | Released | 2019-11-18
2.2.6 | Released | 2019-10-15
2.2.0 | Released | 2019-02-07
2.1.4 | Released | 2018-11-29

## Nightly Builds

To use nightly builds from our Azure DevOps feed, add a `NuGet.config` file in your solution root with the following contents:

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

### 1. Recommended Server CharSet

We recommend to set `utf8mb4` as your MySQL database default charset. This is the default in MySQL 8. The following statement will check your current DB charset:

```sql
show variables like 'character_set_database';
```

### 2. CSPROJ Configuration

Ensure that your `.csproj` file contains the following reference:

```xml
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="3.0.1" />
```

### 3. Services Configuration

Add `Pomelo.EntityFrameworkCore.MySql` to the services configuration in your the `Startup.cs` file.

```csharp
using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

// replace "YourNamespace" with the namespace of your application
namespace YourNamespace
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // other service configurations go here
            // replace "YourDbContext" with the class name of your DbContext
            services.AddDbContextPool<YourDbContext>(options => options
                // replace with your connection string
                .UseMySql("Server=localhost;Database=ef;User=root;Password=1234;", mySqlOptions => mySqlOptions
                    // replace with your Server Version and Type
                    .ServerVersion(new ServerVersion(new Version(8, 0, 18), ServerType.MySql))
            ));
        }
    }
}
```

View our [Configuration Options Wiki Page](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/wiki/Configuration-Options) for a complete list of supported options.

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

One of the easiest ways to contribute is to report issues and participate in discussions on issues. You can also contribute by submitting pull requests with code changes and supporting tests.

`Pomelo.EntityFrameworkCore.MySql` is always looking for additional core contributors. If you got a couple of hours a week and know your way around EF Core, give us a nudge.

## License

[MIT](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/blob/master/LICENSE)
