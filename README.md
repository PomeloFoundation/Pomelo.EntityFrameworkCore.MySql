# Pomelo.EntityFrameworkCore.MySql

[![Travis build status](https://img.shields.io/travis/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql.svg?label=travis-ci&branch=master&style=flat-square)](https://travis-ci.org/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)
[![AppVeyor build status](https://img.shields.io/appveyor/ci/Kagamine/Pomelo-EntityFrameworkCore-MySql/master.svg?label=appveyor&style=flat-square)](https://ci.appveyor.com/project/Kagamine/pomelo-entityframeworkcore-mysql/branch/master) [![NuGet](https://img.shields.io/nuget/v/Pomelo.EntityFrameworkCore.MySql.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/) [![MyGet](https://img.shields.io/myget/pomelo/vpre/Pomelo.EntityFrameworkCore.MySql.svg?style=flat-square&label=myget)](https://www.myget.org/Package/Details/pomelo?packageType=nuget&packageId=Pomelo.EntityFrameworkCore.MySql) [![Join the chat at https://gitter.im/PomeloFoundation/Home](https://badges.gitter.im/PomeloFoundation/Home.svg)](https://gitter.im/PomeloFoundation/Home?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Pomelo.EntityFrameworkCore.MySql is an Entity Framework Core provider built on top of [MySqlConnector](https://github.com/mysql-net/MySqlConnector). It enables use the Entity Framework Core ORM with MySQL.  Async functions in this library properly implement Async I/O at the lowest level, unlike providers based on Oracle's MySql.Data library which uses Sync I/O at the lowest level.

## Nightly Builds

To add a `NuGet.config` file in your solution root, then you can use the unstable packages:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="Pomelo" value="https://www.myget.org/F/pomelo/api/v3/index.json" />
    <add key="nuget.org" value="https://www.nuget.org/api/v2" />
  </packageSources>
</configuration>
```

## Getting Started

Here is a console application sample for accessing a MySQL database using Entity Framework:

① We recommand you to set `utf8mb4` as your MySQL database default charset. The following statement will check your DB charset.
```sql
show variables like 'character_set_database';
```

② Put `Pomelo.EntityFrameworkCore.MySql` into your project's `.csproj` file
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp2.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="2.0.0-rtm-*" />
    <PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="2.0.0-rtm-*" />
  </ItemGroup>

  <ItemGroup>
    <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.0" />
  </ItemGroup>
  
</Project>
```

③ Implement some models, DbContext in `Program.cs`. Then overriding the OnConfiguring of DbContext to use MySQL database. Besides, you can define a JsonObject<T> field if you are using MySQL Server 5.7. Finally to invoking MySQL with EF Core in your Main() method.

```C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MySqlTest
{
    public class User
    {
        public int UserId { get; set; }

        [MaxLength(64)]
        public string Name { get; set; }
    }

    public class Blog
    {
        public Guid Id { get; set; }

        [MaxLength(32)]
        public string Title { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public string Content { get; set; }

        public JsonObject<List<string>> Tags { get; set; } // Json storage (MySQL 5.7 only)
    }

    public class MyContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseMySql(@"Server=localhost;database=ef;uid=root;pwd=123456;");
    }

    public class Program
    {
        public static void Main()
        {
            using (var context = new MyContext())
            {
                // Create database
                context.Database.EnsureCreated();

                // Init sample data
                var user = new User { Name = "Yuuko" };
                context.Add(user);
                var blog1 = new Blog {
                    Title = "Title #1",
                    UserId = user.UserId,
                    Tags = new List<string>() { "ASP.NET Core", "MySQL", "Pomelo" }
                };
                context.Add(blog1);
                var blog2 = new Blog
                {
                    Title = "Title #2",
                    UserId = user.UserId,
                    Tags = new List<string>() { "ASP.NET Core", "MySQL" }
                };
                context.Add(blog2);
                context.SaveChanges();

                // Changing and save json object #1
                blog1.Tags.Object.Clear();
                context.SaveChanges();

                // Changing and save json object #2
                blog1.Tags.Object.Add("Pomelo");
                context.SaveChanges();

                // Output data
                var ret = context.Blogs
                    .Where(x => x.Tags.Object.Contains("Pomelo"))
                    .ToList();
                foreach (var x in ret)
                {
                    Console.WriteLine($"{ x.Id } { x.Title }");
                    Console.Write("[Tags]: ");
                    foreach(var y in x.Tags.Object)
                        Console.Write(y + " ");
                    Console.WriteLine();
                }
            }
            Console.Read();
        }
    }
}
```

By viewing the following full project which is a single-user blog system and based on this library(MySQL for Entity Framework Core) to explorer more features: [View on GitHub](https://github.com/kagamine/yuukoblog-netcore-mysql).

## Schedule and Roadmap

Milestone | Release week
----------|-------------
2.0.0-preview2-final     | July 26th 2017
2.0.0 | September 4th, 2017

### 2.0.0-rtm

Support full text and spatial indexes.

### 2.0.0-preview2

Compatible with Entity Framework Core 2.0.0-preview2-final.

#### Scaffolding Tutorial

Using the tool to execute scaffolding commands
```
dotnet ef dbcontext scaffold "Server=localhost;User Id=root;Password=123456;Database=eftests" "Pomelo.EntityFrameworkCore.MySql"
```

## Contribute

One of the easiest ways to contribute is to participate in discussions and discuss issues. You can also contribute by submitting pull requests with code changes.

## License

[MIT](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/blob/master/LICENSE)
