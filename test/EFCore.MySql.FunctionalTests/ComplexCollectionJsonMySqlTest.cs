// Copyright (c) Microting. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microting.EntityFrameworkCore.MySql.Infrastructure;
using Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests;

/// <summary>
/// Tests for complex collections (collections of complex types) mapped to JSON columns.
/// This is required in EF Core 10+ for complex collections.
/// 
/// Requirements:
/// - MySQL 5.7.8+ (JSON type support)
/// - MariaDB 10.2.7+ (JSON alias for LONGTEXT with JSON_VALID constraint)
/// </summary>
[SupportedServerVersionCondition("Json")]
public class ComplexCollectionJsonMySqlTest : IClassFixture<ComplexCollectionJsonMySqlTest.ComplexCollectionJsonMySqlFixture>
{
    private readonly ComplexCollectionJsonMySqlFixture _fixture;

    public ComplexCollectionJsonMySqlTest(ComplexCollectionJsonMySqlFixture fixture)
    {
        _fixture = fixture;
    }

    [ConditionalFact]
    public virtual async Task Can_insert_and_read_complex_collection()
    {
        using var context = _fixture.CreateContext();
        
        var school = new School
        {
            Id = 1,
            Name = "Test University",
            Departments = new List<Department>
            {
                new Department { Name = "Computer Science", Budget = 100000 },
                new Department { Name = "Mathematics", Budget = 80000 }
            }
        };

        context.Schools.Add(school);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var retrieved = await context.Schools.FirstAsync(s => s.Id == 1);
        
        Assert.Equal("Test University", retrieved.Name);
        Assert.Equal(2, retrieved.Departments.Count);
        Assert.Equal("Computer Science", retrieved.Departments[0].Name);
        Assert.Equal(100000, retrieved.Departments[0].Budget);
        Assert.Equal("Mathematics", retrieved.Departments[1].Name);
        Assert.Equal(80000, retrieved.Departments[1].Budget);
    }

    [ConditionalFact]
    public virtual async Task Can_query_complex_collection_property()
    {
        using var context = _fixture.CreateContext();
        
        var school = new School
        {
            Id = 2,
            Name = "State College",
            Departments = new List<Department>
            {
                new Department { Name = "Engineering", Budget = 150000 },
                new Department { Name = "Physics", Budget = 90000 }
            }
        };

        context.Schools.Add(school);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        // Note: Querying into complex collections may have limitations depending on the database
        var result = await context.Schools
            .Where(s => s.Name == "State College")
            .Select(s => s.Departments)
            .FirstOrDefaultAsync();
        
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    public class School
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        // Complex collection - MUST be mapped to JSON in EF Core 10+
        public List<Department> Departments { get; set; }
    }

    public class Department
    {
        public string Name { get; set; }
        public int Budget { get; set; }
    }

    public class ComplexCollectionJsonMySqlContext : DbContext
    {
        public ComplexCollectionJsonMySqlContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<School> Schools { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<School>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // In EF Core 10, complex collections MUST be mapped to JSON columns
                entity.ComplexCollection(e => e.Departments).ToJson();
            });
        }
    }

    public class ComplexCollectionJsonMySqlFixture : SharedStoreFixtureBase<ComplexCollectionJsonMySqlContext>
    {
        protected override string StoreName => "ComplexCollectionJsonTest";
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

        public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
        {
            var optionsBuilder = base.AddOptions(builder);
            new MySqlDbContextOptionsBuilder(optionsBuilder).EnablePrimitiveCollectionsSupport();
            return optionsBuilder;
        }
    }
}
