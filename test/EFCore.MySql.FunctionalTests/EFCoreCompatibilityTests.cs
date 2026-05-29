// // Copyright (c) Microting. All rights reserved.
// // Licensed under the MIT. See LICENSE in the project root for license information.
//
// using System;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using Microting.EntityFrameworkCore.MySql.Infrastructure.Internal;
// using Xunit;
//
// namespace Microting.EntityFrameworkCore.MySql.FunctionalTests
// {
//     /// <summary>
//     /// Tests demonstrating EF Core version compatibility patterns,
//     /// particularly for the ExecuteUpdate API changes in EF Core 10.
//     /// </summary>
//     public class EFCoreCompatibilityTests : IClassFixture<EFCoreCompatibilityTests.CompatibilityTestFixture>
//     {
//         private readonly CompatibilityTestFixture _fixture;
//
//         public EFCoreCompatibilityTests(CompatibilityTestFixture fixture)
//         {
//             _fixture = fixture;
//         }
//
//         [Fact]
//         public void EFCoreVersion_IsDetected()
//         {
//             // Verify that version detection works
//             var version = Microting.EntityFrameworkCore.MySql.Infrastructure.Internal.EFCoreCompatibilityHelper.EFCoreVersion;
//             Assert.NotNull(version);
//             Assert.True(version.Major >= 8, "Should be using EF Core 8 or greater");
//         }
//
//         [Fact]
//         public void VersionFlags_AreSetCorrectly()
//         {
//             // Test version detection flags
//             var isEFCore8OrGreater = EFCoreCompatibilityHelper.IsEFCore8OrGreater;
//             var isEFCore9OrGreater = EFCoreCompatibilityHelper.IsEFCore9OrGreater;
//             var isEFCore10OrGreater = EFCoreCompatibilityHelper.IsEFCore10OrGreater;
//
//             Assert.True(isEFCore8OrGreater, "Should support EF Core 8+");
//
//             // These will be true based on the actual EF Core version being used
//             if (EFCoreCompatibilityHelper.EFCoreVersion.Major >= 9)
//             {
//                 Assert.True(isEFCore9OrGreater);
//             }
//
//             if (EFCoreCompatibilityHelper.EFCoreVersion.Major >= 10)
//             {
//                 Assert.True(isEFCore10OrGreater);
//             }
//         }
//
//         [Fact]
//         public async Task ExecuteUpdate_SingleProperty_CompatibilityPattern()
//         {
//             using var context = _fixture.CreateContext();
//
//             // Setup test data
//             var entity = new TestEntity { Name = "Original", Value = 42 };
//             context.TestEntities.Add(entity);
//             await context.SaveChangesAsync();
//
//             // Test version-specific ExecuteUpdate patterns
// #if EFCORE10_OR_GREATER
//             // EF Core 10+ pattern - Action-based setter
//             await context.TestEntities
//                 .Where(e => e.Id == entity.Id)
//                 .ExecuteUpdateAsync(e => e.Name = "Updated via EF Core 10+");
// #else
//             // EF Core 9 and earlier pattern - Expression-based setter
//             await context.TestEntities
//                 .Where(e => e.Id == entity.Id)
//                 .ExecuteUpdateAsync(setters => setters.SetProperty(e => e.Name, "Updated via EF Core 9-"));
// #endif
//
//             // Verify the update worked
//             var updatedEntity = await context.TestEntities.FindAsync(entity.Id);
//             Assert.NotNull(updatedEntity);
//
// #if EFCORE10_OR_GREATER
//             Assert.Equal("Updated via EF Core 10+", updatedEntity.Name);
// #else
//             Assert.Equal("Updated via EF Core 9-", updatedEntity.Name);
// #endif
//         }
//
//         [Fact]
//         public async Task ExecuteUpdate_MultipleProperties_CompatibilityPattern()
//         {
//             using var context = _fixture.CreateContext();
//
//             // Setup test data
//             var entity = new TestEntity { Name = "Original", Value = 42, LastModified = DateTime.MinValue };
//             context.TestEntities.Add(entity);
//             await context.SaveChangesAsync();
//
//             var updateTime = DateTime.UtcNow;
//
//             // Test version-specific ExecuteUpdate patterns for multiple properties
// #if EFCORE10_OR_GREATER
//             // EF Core 10+ pattern - Object initializer syntax
//             await context.TestEntities
//                 .Where(e => e.Id == entity.Id)
//                 .ExecuteUpdateAsync(e => new TestEntity
//                 {
//                     Name = "Multi-Updated EF10+",
//                     Value = e.Value * 2,
//                     LastModified = updateTime
//                 });
// #else
//             // EF Core 9 and earlier pattern - Chained SetProperty calls
//             await context.TestEntities
//                 .Where(e => e.Id == entity.Id)
//                 .ExecuteUpdateAsync(setters => setters
//                     .SetProperty(e => e.Name, "Multi-Updated EF9-")
//                     .SetProperty(e => e.Value, e => e.Value * 2)
//                     .SetProperty(e => e.LastModified, updateTime));
// #endif
//
//             // Verify the updates worked
//             var updatedEntity = await context.TestEntities.FindAsync(entity.Id);
//             Assert.NotNull(updatedEntity);
//
// #if EFCORE10_OR_GREATER
//             Assert.Equal("Multi-Updated EF10+", updatedEntity.Name);
// #else
//             Assert.Equal("Multi-Updated EF9-", updatedEntity.Name);
// #endif
//             Assert.Equal(84, updatedEntity.Value); // 42 * 2
//             Assert.True(Math.Abs((updatedEntity.LastModified - updateTime).TotalSeconds) < 1);
//         }
//
//         [Fact]
//         public void CreateExecuteUpdateSetter_ReturnsCorrectType()
//         {
//             // Test the compatibility helper method
//             if (EFCoreCompatibilityHelper.IsEFCore10OrGreater)
//             {
//                 // For EF Core 10+, should return Action<T>
//                 Action<TestEntity> action = e => e.Name = "test";
//                 var result = EFCoreCompatibilityHelper.CreateExecuteUpdateSetter<TestEntity>(setterAction: action);
//                 Assert.IsType<Action<TestEntity>>(result);
//             }
//             else
//             {
//                 // For EF Core 9-, should return Expression<Func<T, T>>
//                 System.Linq.Expressions.Expression<Func<TestEntity, TestEntity>> expression =
//                     e => new TestEntity { Name = "test" };
//                 var result = EFCoreCompatibilityHelper.CreateExecuteUpdateSetter<TestEntity>(setterExpression: expression);
//                 Assert.IsType<System.Linq.Expressions.Expression<Func<TestEntity, TestEntity>>>(result);
//             }
//         }
//
//         public class CompatibilityTestFixture : IDisposable
//         {
//             private const string ConnectionString = "Server=localhost;Database=pomelo_test_compatibility;Uid=root;Pwd=;";
//
//             public TestContext CreateContext()
//             {
//                 var options = new DbContextOptionsBuilder<TestContext>()
//                     .UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString))
//                     .Options;
//
//                 var context = new TestContext(options);
//                 context.Database.EnsureCreated();
//                 return context;
//             }
//
//             public void Dispose()
//             {
//                 using var context = CreateContext();
//                 context.Database.EnsureDeleted();
//             }
//         }
//
//         public class TestContext : DbContext
//         {
//             public TestContext(DbContextOptions<TestContext> options) : base(options) { }
//
//             public DbSet<TestEntity> TestEntities { get; set; }
//
//             protected override void OnModelCreating(ModelBuilder modelBuilder)
//             {
//                 modelBuilder.Entity<TestEntity>(entity =>
//                 {
//                     entity.HasKey(e => e.Id);
//                     entity.Property(e => e.Name).HasMaxLength(100);
//                 });
//             }
//         }
//
//         public class TestEntity
//         {
//             public int Id { get; set; }
//             public string Name { get; set; }
//             public int Value { get; set; }
//             public DateTime LastModified { get; set; }
//         }
//     }
// }
