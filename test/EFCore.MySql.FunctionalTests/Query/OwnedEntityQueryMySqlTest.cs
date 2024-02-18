using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class OwnedEntityQueryMySqlTest : OwnedEntityQueryRelationalTestBase
    {
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

        public override async Task Multiple_single_result_in_projection_containing_owned_types(bool async)
        {
            await base.Multiple_single_result_in_projection_containing_owned_types(async);

        AssertSql(
"""
SELECT `e`.`Id`, `c2`.`Id`, `c2`.`EntityId`, `c2`.`Owned_IsDeleted`, `c2`.`Owned_Value`, `c2`.`Type`, `c2`.`c`, `c4`.`Id`, `c4`.`EntityId`, `c4`.`Owned_IsDeleted`, `c4`.`Owned_Value`, `c4`.`Type`, `c4`.`c`
FROM `Entities` AS `e`
LEFT JOIN (
    SELECT `c1`.`Id`, `c1`.`EntityId`, `c1`.`Owned_IsDeleted`, `c1`.`Owned_Value`, `c1`.`Type`, `c1`.`c`
    FROM (
        SELECT `c`.`Id`, `c`.`EntityId`, `c`.`Owned_IsDeleted`, `c`.`Owned_Value`, `c`.`Type`, 1 AS `c`, ROW_NUMBER() OVER(PARTITION BY `c`.`EntityId` ORDER BY `c`.`EntityId`, `c`.`Id`) AS `row`
        FROM `Child` AS `c`
        WHERE `c`.`Type` = 1
    ) AS `c1`
    WHERE `c1`.`row` <= 1
) AS `c2` ON `e`.`Id` = `c2`.`EntityId`
LEFT JOIN (
    SELECT `c3`.`Id`, `c3`.`EntityId`, `c3`.`Owned_IsDeleted`, `c3`.`Owned_Value`, `c3`.`Type`, `c3`.`c`
    FROM (
        SELECT `c0`.`Id`, `c0`.`EntityId`, `c0`.`Owned_IsDeleted`, `c0`.`Owned_Value`, `c0`.`Type`, 1 AS `c`, ROW_NUMBER() OVER(PARTITION BY `c0`.`EntityId` ORDER BY `c0`.`EntityId`, `c0`.`Id`) AS `row`
        FROM `Child` AS `c0`
        WHERE `c0`.`Type` = 2
    ) AS `c3`
    WHERE `c3`.`row` <= 1
) AS `c4` ON `e`.`Id` = `c4`.`EntityId`
""");
        }

        public override async Task Owned_collection_basic_split_query(bool async)
        {
            // Use custom context to set prefix length, so we don't exhaust the max. key length.
            var contextFactory = await InitializeAsync<Context25680>(onModelCreating: modelBuilder =>
            {
                modelBuilder.Entity<Location25680>().OwnsMany(e => e.PublishTokenTypes,
                    b =>
                    {
                        b.WithOwner(e => e.Location).HasForeignKey(e => e.LocationId);
                        b.HasKey(e => new { e.LocationId, e.ExternalId, e.VisualNumber, e.TokenGroupId })
                            .HasPrefixLength(0, 128, 128, 128); // <-- set prefix length, so we don't exhaust the max. key length
                    });
            });

            using var context = contextFactory.CreateContext();

            var id = new Guid("6c1ae3e5-30b9-4c77-8d98-f02075974a0a");
            var query = context.Set<Location25680>().Where(e => e.Id == id).AsSplitQuery();
            var result = async
                ? await query.FirstOrDefaultAsync()
                : query.FirstOrDefault();
        }

        [ConditionalTheory(Skip = "https://github.com/dotnet/efcore/pull/32509#issuecomment-1948812777")]
        public override /*async*/ Task Projecting_correlated_collection_property_for_owned_entity(bool async)
        {
            return Task.CompletedTask;
            // var contextFactory = await InitializeAsync<Context18582>(seed: c => c.Seed());
            //
            // using var context = contextFactory.CreateContext();
            // var query = context.Warehouses.Select(
            //     x => new Context18582.WarehouseModel
            //     {
            //         WarehouseCode = x.WarehouseCode,
            //         DestinationCountryCodes = x.DestinationCountries.Select(c => c.CountryCode)
            //             .OrderByDescending(c => c) // <-- explicitly order
            //             .ToArray()
            //     }).AsNoTracking();
            //
            // var result = async
            //     ? await query.ToListAsync()
            //     : query.ToList();
            //
            // var warehouseModel = Assert.Single(result);
            // Assert.Equal("W001", warehouseModel.WarehouseCode);
            // Assert.True(new[] { "US", "CA" }.SequenceEqual(warehouseModel.DestinationCountryCodes));
        }
    }
}
