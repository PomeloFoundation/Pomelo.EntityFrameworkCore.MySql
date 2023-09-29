using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests;

public class ComplexTypesTrackingMySqlTest : ComplexTypesTrackingTestBase<ComplexTypesTrackingMySqlTest.MySqlFixture>
{
    public ComplexTypesTrackingMySqlTest(MySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        fixture.TestSqlLoggerFactory.Clear();
        fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
        => facade.UseTransaction(transaction.GetDbTransaction());

    public class MySqlFixture : FixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ListLoggerFactory;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            // modelBuilder.Entity<Pub>(
            //     b =>
            //     {
            //         b.ComplexProperty(
            //             e => e.LunchtimeActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e!.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e!.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e!.Notes);
            //             });
            //         b.ComplexProperty(
            //             e => e.EveningActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e.Notes);
            //             });
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //     });
            //
            // modelBuilder.Entity<PubWithStructs>(
            //     b =>
            //     {
            //         b.ComplexProperty(
            //             e => e.LunchtimeActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e!.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e!.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e!.Notes);
            //             });
            //         b.ComplexProperty(
            //             e => e.EveningActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e.Notes);
            //             });
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //     });
            //
            // modelBuilder.Entity<PubWithReadonlyStructs>(
            //     b =>
            //     {
            //         b.ComplexProperty(
            //             e => e.LunchtimeActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e!.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e!.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e!.Notes);
            //             });
            //         b.ComplexProperty(
            //             e => e.EveningActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e.Notes);
            //             });
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //     });
            //
            // modelBuilder.Entity<PubWithRecords>(
            //     b =>
            //     {
            //         b.ComplexProperty(
            //             e => e.LunchtimeActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e!.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e!.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e!.Notes);
            //             });
            //         b.ComplexProperty(
            //             e => e.EveningActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e.Notes);
            //             });
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //     });
            //
            // modelBuilder.Entity<FieldPub>(
            //     b =>
            //     {
            //         b.ComplexProperty(
            //             e => e.LunchtimeActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e!.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e!.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e!.Notes);
            //             });
            //         b.ComplexProperty(
            //             e => e.EveningActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e.Notes);
            //             });
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //     });
            //
            // modelBuilder.Entity<FieldPubWithStructs>(
            //     b =>
            //     {
            //         b.ComplexProperty(
            //             e => e.LunchtimeActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e!.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e!.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e!.Notes);
            //             });
            //         b.ComplexProperty(
            //             e => e.EveningActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e.Notes);
            //             });
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //     });
            //
            // // TODO: Allow binding of complex properties to constructors
            // // modelBuilder.Entity<FieldPubWithReadonlyStructs>(
            // //     b =>
            // //     {
            // //         b.ComplexProperty(
            // //             e => e.LunchtimeActivity, b =>
            // //             {
            // //                 b.ComplexProperty(e => e!.Champions, b => b.Ignore(e => e.Members));
            // //                 b.ComplexProperty(e => e!.RunnersUp, b => b.Ignore(e => e.Members));
            // //                 b.Ignore(e => e!.Notes);
            // //             });
            // //         b.ComplexProperty(
            // //             e => e.EveningActivity, b =>
            // //             {
            // //                 b.ComplexProperty(e => e.Champions, b => b.Ignore(e => e.Members));
            // //                 b.ComplexProperty(e => e.RunnersUp, b => b.Ignore(e => e.Members));
            // //                 b.Ignore(e => e.Notes);
            // //             });
            // //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            // //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            // //     });
            //
            // modelBuilder.Entity<FieldPubWithRecords>(
            //     b =>
            //     {
            //         b.ComplexProperty(
            //             e => e.LunchtimeActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e!.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e!.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e!.Notes);
            //             });
            //         b.ComplexProperty(
            //             e => e.EveningActivity, b =>
            //             {
            //                 b.ComplexProperty(e => e.Champions, b => b.Ignore(e => e.Members));
            //                 b.ComplexProperty(e => e.RunnersUp, b => b.Ignore(e => e.Members));
            //                 b.Ignore(e => e.Notes);
            //             });
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //         b.ComplexProperty(e => e.FeaturedTeam, b => b.Ignore(e => e.Members));
            //     });

            // // TODO: We currently don't support primitive collections yet.
            // var entityTypes = modelBuilder.Model.GetEntityTypes().ToArray();
            // var unprocessedComplexProperties = new Queue<IMutableComplexProperty>(entityTypes.SelectMany(e => e.GetComplexProperties()));
            // var processedComplexProperties = new HashSet<IMutableComplexProperty>();
            //
            // while (unprocessedComplexProperties.TryDequeue(out var complexProperty))
            // {
            //     processedComplexProperties.Add(complexProperty);
            //
            //     Debug.WriteLine(complexProperty.ClrType.Name);
            //
            //     var unmappedProperties = complexProperty.ComplexType.ClrType
            //         .GetProperties()
            //         .Except(
            //             complexProperty.ComplexType
            //                 .GetProperties()
            //                 .Where(p => p.PropertyInfo is not null)
            //                 .Select(p => p.PropertyInfo));
            //
            //     foreach (var property in unmappedProperties)
            //     {
            //
            //         // if (propertyInfo.IsGenericType &&
            //         //     propertyInfo.GetGenericTypeDefinition() == typeof(List<>))
            //         // {
            //         //     Debug.WriteLine($"{propertyInfo.Name} {complexProperty.ClrType.Name}.{property.Name}");
            //         // }
            //     }
            //
            //     foreach (var innerComplexProperty in complexProperty.ComplexType.GetComplexProperties())
            //     {
            //         if (!processedComplexProperties.Contains(innerComplexProperty) &&
            //             !unprocessedComplexProperties.Contains(innerComplexProperty))
            //         {
            //             unprocessedComplexProperties.Enqueue(innerComplexProperty);
            //         }
            //     }
            // }
        }
    }
}
