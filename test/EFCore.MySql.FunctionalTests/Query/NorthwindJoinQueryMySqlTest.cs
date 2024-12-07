using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindJoinQueryMySqlTest : NorthwindJoinQueryRelationalTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindJoinQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        /// <summary>
        /// Needs explicit ordering of views to work consistently with MySQL and MariaDB.
        /// But since CustomerViewModel is private, we can't even override the test case properly.
        /// </summary>
        [ConditionalTheory(Skip = "Needs explicit ordering of views to work consistently with MySQL and MariaDB.")]
        public override async Task SelectMany_with_client_eval_with_constructor(bool async)
        {
            // await AssertQuery(
            //     async,
            //     ss => ss.Set<Customer>()
            //         .Where(c => c.CustomerID.StartsWith("A"))
            //         .OrderBy(c => c.CustomerID)
            //         .Select(
            //             c => new CustomerViewModel(
            //                 c.CustomerID, c.City,
            //                 c.Orders.SelectMany(
            //                         o => o.OrderDetails
            //                             .Where(od => od.OrderID < 11000)
            //                             .Select(od => new OrderDetailViewModel(od.OrderID, od.ProductID)))
            //                     .ToArray())),
            //     assertOrder: true);

            await base.SelectMany_with_client_eval_with_constructor(async);

            AssertSql(
                @"SELECT `c`.`CustomerID`, `c`.`City`, `t0`.`OrderID`, `t0`.`ProductID`, `t0`.`OrderID0`
FROM `Customers` AS `c`
LEFT JOIN (
    SELECT `t`.`OrderID`, `t`.`ProductID`, `o`.`OrderID` AS `OrderID0`, `o`.`CustomerID`
    FROM `Orders` AS `o`
    INNER JOIN (
        SELECT `o0`.`OrderID`, `o0`.`ProductID`
        FROM `Order Details` AS `o0`
        WHERE `o0`.`OrderID` < 11000
    ) AS `t` ON `o`.`OrderID` = `t`.`OrderID`
) AS `t0` ON `c`.`CustomerID` = `t0`.`CustomerID`
WHERE `c`.`CustomerID` LIKE 'A%'
ORDER BY `c`.`CustomerID`, `t0`.`OrderID0`, `t0`.`OrderID`");
        }

        // https://github.com/npgsql/efcore.pg/issues/2759
        // public override Task Join_local_collection_int_closure_is_cached_correctly(bool async)
        //     => Assert.ThrowsAsync<InvalidOperationException>(() => base.Join_local_collection_int_closure_is_cached_correctly(async));
        public override async Task Join_local_collection_int_closure_is_cached_correctly(bool async)
        {
            if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
            {
                await base.Join_local_collection_int_closure_is_cached_correctly(async);
            }
            else
            {
                await Assert.ThrowsAsync<InvalidOperationException>(()
                    => base.Join_local_collection_int_closure_is_cached_correctly(async));
            }
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
