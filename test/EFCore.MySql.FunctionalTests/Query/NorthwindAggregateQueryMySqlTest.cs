using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindAggregateQueryMySqlTest : NorthwindAggregateOperatorsQueryRelationalTestBase<
        NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindAggregateQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override async Task Sum_with_coalesce(bool async)
        {
            await base.Sum_with_coalesce(async);

            AssertSql(
                @"SELECT COALESCE(SUM(COALESCE(`p`.`UnitPrice`, 0.0)), 0.0)
FROM `Products` AS `p`
WHERE `p`.`ProductID` < 40");
        }

        public override async Task Average_over_max_subquery_is_client_eval(bool async)
        {
            await AssertAverage(
                async,
                ss => ss.Set<Customer>().OrderBy(c => c.CustomerID).Take(3),
                selector: c => (decimal)c.Orders.Average(o => 5 + o.OrderDetails.Max(od => od.ProductID)),
                asserter: (a, b) => Assert.Equal(a, b, 12)); // added flouting point precision tolerance

            AssertSql(
                @"@__p_0='3'

SELECT AVG(CAST((
    SELECT AVG(CAST(5 + (
        SELECT MAX(`o0`.`ProductID`)
        FROM `Order Details` AS `o0`
        WHERE `o`.`OrderID` = `o0`.`OrderID`) AS double))
    FROM `Orders` AS `o`
    WHERE `t`.`CustomerID` = `o`.`CustomerID`) AS decimal(65,30)))
FROM (
    SELECT `c`.`CustomerID`
    FROM `Customers` AS `c`
    ORDER BY `c`.`CustomerID`
    LIMIT @__p_0
) AS `t`");
        }

        public override async Task Average_over_nested_subquery_is_client_eval(bool async)
        {
            await AssertAverage(
                async,
                ss => ss.Set<Customer>().OrderBy(c => c.CustomerID).Take(3),
                selector: c => (decimal)c.Orders.Average(o => 5 + o.OrderDetails.Average(od => od.ProductID)),
                asserter: (a, b) => Assert.Equal(a, b, 12)); // added flouting point precision tolerance

            AssertSql(
                @"@__p_0='3'

SELECT AVG(CAST((
    SELECT AVG(5.0 + (
        SELECT AVG(CAST(`o0`.`ProductID` AS double))
        FROM `Order Details` AS `o0`
        WHERE `o`.`OrderID` = `o0`.`OrderID`))
    FROM `Orders` AS `o`
    WHERE `t`.`CustomerID` = `o`.`CustomerID`) AS decimal(65,30)))
FROM (
    SELECT `c`.`CustomerID`
    FROM `Customers` AS `c`
    ORDER BY `c`.`CustomerID`
    LIMIT @__p_0
) AS `t`");
        }

        public override async Task Contains_with_local_anonymous_type_array_closure(bool async)
        {
            // Aggregates. Issue #15937.
            await AssertTranslationFailed(() => base.Contains_with_local_anonymous_type_array_closure(async));

            AssertSql();
        }

        public override async Task Contains_with_local_tuple_array_closure(bool async)
            => await AssertTranslationFailed(() => base.Contains_with_local_tuple_array_closure(async));

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
