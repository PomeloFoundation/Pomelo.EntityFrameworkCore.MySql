# Pomelo.EntityFrameworkCore.MySql

[![Travis build status](https://img.shields.io/travis/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql.svg?label=travis-ci&branch=master&style=flat-square)](https://travis-ci.org/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)
[![AppVeyor build status](https://img.shields.io/appveyor/ci/Kagamine/Pomelo-EntityFrameworkCore-MySql/master.svg?label=appveyor&style=flat-square)](https://ci.appveyor.com/project/Kagamine/pomelo-entityframeworkcore-mysql/branch/master)
[![NuGet](https://img.shields.io/nuget/v/Pomelo.EntityFrameworkCore.MySql.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/)
[![MyGet](https://img.shields.io/myget/pomelo/vpre/Pomelo.EntityFrameworkCore.MySql.svg?style=flat-square&label=myget)](https://www.myget.org/Package/Details/pomelo?packageType=nuget&packageId=Pomelo.EntityFrameworkCore.MySql)
[![Join the chat at https://gitter.im/PomeloFoundation/Home](https://badges.gitter.im/PomeloFoundation/Home.svg)](https://gitter.im/PomeloFoundation/Home?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

`Pomelo.EntityFrameworkCore.MySql` is an Entity Framework Core provider built on top of [MySqlConnector](https://github.com/mysql-net/MySqlConnector) that enables the use of the Entity Framework Core ORM with MySQL.

**Pomelo.EntityFrameworkCore.MySql is currently [looking for core contributors](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/522)**

## Nightly Builds

To use nightly builds from our MyGet feed, add a `NuGet.config` file in your solution root with the following contents:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="Pomelo" value="https://www.myget.org/F/pomelo/api/v3/index.json" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

## Getting Started

### 1. Recommended Server CharSet

We recommend you to set `utf8mb4` as your MySQL database default charset. The following statement will check your DB charset:

```sql
show variables like 'character_set_database';
```

### 2. CSPROJ Configuration

Ensure that your `.csproj` file has the following references.

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp2.1</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.All" Version="2.1.1" />
    <PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="2.1.1" />
  </ItemGroup>
  
</Project>
```

### 3. Services Configuration

Add `Pomelo.EntityFrameworkCore.MySql` to the services configuration in your the `Startup.cs` file.

```csharp
using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace YourNamespace // replace "YourNamespace" with the namespace of your application
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // other service configurations go here
            services.AddDbContextPool<YourDbContext>( // replace "YourDbContext" with the class name of your DbContext
                options => options.UseMySql("Server=localhost;Database=ef;User=root;Password=123456;", // replace with your Connection String
                    mysqlOptions =>
                    {
                        mysqlOptions.ServerVersion(new Version(5, 7, 17), ServerType.MySql); // replace with your Server Version and Type
                    }
            ));
        }
    }
}
```

View our [MySql Provider Configuration Options Wiki Page](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/wiki/MySql-Provider-Configuration-Options) for a complete list of supported options.

### 4. Sample Application

Check out our [Integration Tests](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/tree/master/test/EFCore.MySql.IntegrationTests) for an example repository that includes a MVC Application.

### 5. Read the EF Core Documentation

Refer to Microsoft's [EF Core Documentation](https://docs.microsoft.com/en-us/ef/core/) for detailed instructions and examples on using EF Core.

## Schedule and Roadmap

Milestone | Release week
----------|-------------
2.1.1 | 7/8/2018

#### Scaffolding Tutorial

Using the tool to execute scaffolding commands:

```
dotnet ef dbcontext scaffold "Server=localhost;Database=ef;User=root;Password=123456;" "Pomelo.EntityFrameworkCore.MySql"
```

## Contribute

One of the easiest ways to contribute is to report issues and participate in discussions on issues. You can also contribute by submitting pull requests with code changes and supporting tests.

## License

[MIT](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/blob/master/LICENSE)
