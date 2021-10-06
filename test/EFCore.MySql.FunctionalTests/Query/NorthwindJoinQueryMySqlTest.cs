using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
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

        protected override bool CanExecuteQueryString
            => true;

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task SelectMany_correlated_subquery_take(bool async)
        {
            return base.SelectMany_correlated_subquery_take(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Distinct_SelectMany_correlated_subquery_take(bool async)
        {
            return base.Distinct_SelectMany_correlated_subquery_take(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Distinct_SelectMany_correlated_subquery_take_2(bool async)
        {
            return base.Distinct_SelectMany_correlated_subquery_take_2(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WindowFunctions))]
        public override Task Take_SelectMany_correlated_subquery_take(bool async)
        {
            return base.Take_SelectMany_correlated_subquery_take(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task SelectMany_with_client_eval(bool async)
        {
            return base.SelectMany_with_client_eval(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task SelectMany_with_client_eval_with_collection_shaper(bool async)
        {
            return base.SelectMany_with_client_eval_with_collection_shaper(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task SelectMany_with_client_eval_with_collection_shaper_ignored(bool async)
        {
            return base.SelectMany_with_client_eval_with_collection_shaper_ignored(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task SelectMany_with_selecting_outer_entity(bool async)
        {
            return base.SelectMany_with_selecting_outer_entity(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task SelectMany_with_selecting_outer_element(bool async)
        {
            return base.SelectMany_with_selecting_outer_element(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CrossApply))]
        public override Task SelectMany_with_selecting_outer_entity_column_and_inner_column(bool async)
        {
            return base.SelectMany_with_selecting_outer_entity_column_and_inner_column(async);
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterApply))]
        public override Task Take_in_collection_projection_with_FirstOrDefault_on_top_level(bool async)
        {
            return base.Take_in_collection_projection_with_FirstOrDefault_on_top_level(async);
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

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
