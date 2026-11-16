using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using NameSpace1;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microting.EntityFrameworkCore.MySql.Tests;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class AdHocMiscellaneousQueryMySqlTest : AdHocMiscellaneousQueryRelationalTestBase
{
    public AdHocMiscellaneousQueryMySqlTest(NonSharedFixture fixture)
        : base(fixture)
    {
    }

    protected override ITestStoreFactory NonSharedTestStoreFactory
        => MySqlTestStoreFactory.Instance;

    protected override DbContextOptionsBuilder SetParameterizedCollectionMode(DbContextOptionsBuilder optionsBuilder, ParameterTranslationMode parameterizedCollectionMode)
    {
        new MySqlDbContextOptionsBuilder(optionsBuilder).UseParameterizedCollectionMode(parameterizedCollectionMode);

        return optionsBuilder;
    }

    // The base test asserts that the query fails to translate with a message containing "requires the SQL APPLY
    // operation". Neither branch holds for this provider:
    //   - On MySQL 8.0.14+ LATERAL is supported, so the query translates successfully and no exception is thrown.
    //   - On MariaDB and older MySQL the query does fail to translate, but with this provider's own
    //     "The LINQ expression 'OUTER APPLY ...' could not be translated" message, which does not contain the
    //     string the base test looks for.
    [Fact(Skip = "The base test asserts an EF Core specific 'requires the SQL APPLY' translation failure that does not apply to MySQL or MariaDB.")]
    public override Task Correlated_SelectMany_DefaultIfEmpty_whole_object()
        => Task.CompletedTask;

    public override async Task Plain_inner_no_aggregate_LeftJoin_whole_object()
    {
        var contextFactory = await InitializeNonSharedTest<Context30915>(seed: Seed30915);
        using var context = contextFactory.CreateDbContext();

        var categories = context.Requests.Select(r => new { r.PickupStatusId, Count = 1 });
        var query = context.Statuses
            .LeftJoin(categories, s => s.PickupStatusId, c => c.PickupStatusId, (s, countInfo) => new { s.PickupStatusId, countInfo })
            .OrderBy(e => e.PickupStatusId);

        await Assert.ThrowsAsync<InvalidOperationException>(() => query.ToListAsync());
    }

    public override async Task RightJoin_whole_object_outer_nullable()
    {
        var contextFactory = await InitializeNonSharedTest<Context30915>(seed: Seed30915);
        using var context = contextFactory.CreateDbContext();

        var categories = context.Requests
            .GroupBy(r => r.PickupStatusId, (k, els) => new { pickupStatusId = k, Count = els.Count() });
        var query = categories
            .RightJoin(
                context.Statuses,
                c => c.pickupStatusId,
                s => s.PickupStatusId,
                (countInfo, s) => new { s.PickupStatusId, countInfo })
            .OrderBy(e => e.PickupStatusId);

        await Assert.ThrowsAsync<InvalidOperationException>(() => query.ToListAsync());
    }

    protected override Task Seed2951(Context2951 context)
        => context.Database.ExecuteSqlRawAsync(
            """
CREATE TABLE `ZeroKey` (`Id` int);
INSERT INTO `ZeroKey` VALUES (NULL)
""");

    protected override async Task Seed30915(Context30915 context)
    {
        context.Statuses.AddRange(
            new Context30915.PickupStatus30915 { PickupStatusId = 1, Name = "Active" },
            new Context30915.PickupStatus30915 { PickupStatusId = 2, Name = "NoRequests" },
            new Context30915.PickupStatus30915 { PickupStatusId = 3, Name = "Busy" });

        context.Requests.AddRange(
            new Context30915.PickupRequest30915 { PickupStatusId = 1, Priority = 5 },
            new Context30915.PickupRequest30915 { PickupStatusId = 1, Priority = null },
            new Context30915.PickupRequest30915 { PickupStatusId = 3, Priority = 7 });

        await context.SaveChangesAsync();
    }

    public override async Task Multiple_different_entity_type_from_different_namespaces(bool async)
    {
        // The only change is the FromSqlRaw SQL string:
        //     Original: SELECT cast(null as int) AS MyValue
        //     Changed:  SELECT cast(null as signed) AS MyValue
        // The other comments are part of the base implementation.

        var contextFactory = await InitializeNonSharedTest<Context23981>();
        using var context = contextFactory.CreateDbContext();
        //var good1 = context.Set<NameSpace1.TestQuery>().FromSqlRaw(@"SELECT 1 AS MyValue").ToList(); // OK
        //var good2 = context.Set<NameSpace2.TestQuery>().FromSqlRaw(@"SELECT 1 AS MyValue").ToList(); // OK
        var bad = context.Set<TestQuery>().FromSqlRaw(@"SELECT cast(null as signed) AS MyValue").ToList(); // Exception
    }
}
