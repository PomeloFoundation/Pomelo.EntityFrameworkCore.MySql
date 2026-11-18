using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindAggregateOperatorsQueryMySqlTest : NorthwindAggregateOperatorsQueryRelationalTestBase<
        NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindAggregateOperatorsQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override Task Min_no_data_subquery(bool async)
            => Assert.ThrowsAsync<InvalidOperationException>(() => AssertQueryScalar(
                async,
                ss => ss.Set<Customer>().Select(c => c.Orders.Where(o => o.OrderID == -1).Min(o => o.OrderID))));

        public override Task Max_no_data_subquery(bool async)
            => Assert.ThrowsAsync<InvalidOperationException>(() => AssertQueryScalar(
                async,
                ss => ss.Set<Customer>().Select(c => c.Orders.Where(o => o.OrderID == -1).Max(o => o.OrderID))));

        public override Task MinBy_no_data_subquery_value_type(bool async)
            => Assert.ThrowsAsync<InvalidOperationException>(() => AssertQuery(
                async,
                ss => ss.Set<Customer>().Select(c => c.Orders.Where(o => o.OrderID == -1).Select(o => o.OrderID).MinBy(o => o))));

        public override Task MaxBy_no_data_subquery_value_type(bool async)
            => Assert.ThrowsAsync<InvalidOperationException>(() => AssertQuery(
                async,
                ss => ss.Set<Customer>().Select(c => c.Orders.Where(o => o.OrderID == -1).Select(o => o.OrderID).MaxBy(o => o))));

        public override Task Average_no_data_subquery(bool async)
            => Assert.ThrowsAsync<InvalidOperationException>(() => AssertQueryScalar(
                async,
                ss => ss.Set<Customer>().Select(c => c.Orders.Where(o => o.OrderID == -1).Average(o => o.OrderID))));

        public override Task Average_over_max_subquery(bool async)
            => AssertAverage(
                async,
                ss => ss.Set<Customer>().OrderBy(c => c.CustomerID).Take(3),
                selector: c => (decimal)c.Orders.Average(o => 5 + o.OrderDetails.Max(od => od.ProductID)),
                asserter: (a, b) => Assert.Equal(a, b, 12)); // added flouting point precision tolerance

        public override Task Average_over_nested_subquery(bool async)
            => AssertAverage(
                async,
                ss => ss.Set<Customer>().OrderBy(c => c.CustomerID).Take(3),
                selector: c => (decimal)c.Orders.Average(o => 5 + o.OrderDetails.Average(od => od.ProductID)),
                asserter: (a, b) => Assert.Equal(a, b, 12)); // added flouting point precision tolerance

        // The base test does MinBy(c => c.Orders.Sum(o => o.OrderID)) over all customers. Northwind contains two
        // customers with no orders at all (FISSA and PARIS), so both tie on a sum of 0 and which one MinBy returns is
        // arbitrary. The expected result is produced in memory, where the first match wins; MariaDB happens to return
        // the other one. The tie makes the test inherently server-dependent, so only run it on MySQL.
        [SupportedServerVersionCondition("0.0.0-mysql", Skip = "Result is ambiguous: two Northwind customers have no orders, so they tie for the minimum.")]
        public override Task MinBy_over_subquery(bool async)
            => base.MinBy_over_subquery(async);

        public override async Task Type_casting_inside_sum(bool async)
        {
            await AssertSum(
                async,
                ss => ss.Set<OrderDetail>(),
                x => (decimal)x.Discount,
                asserter: (a, b) => Assert.Equal(a, b, 3)); // added flouting point precision tolerance

            AssertSql(
"""
SELECT COALESCE(SUM(CAST(`o`.`Discount` AS decimal(65,30))), 0.0)
FROM `Order Details` AS `o`
""");
        }

        // TODO: Implement TranslatePrimitiveCollection.
        public override async Task Contains_with_local_anonymous_type_array_closure(bool async)
        {
            // Aggregates. Issue #15937.
            // await AssertTranslationFailed(() => base.Contains_with_local_anonymous_type_array_closure(async));

            await Assert.ThrowsAsync<InvalidOperationException>(() => base.Contains_with_local_anonymous_type_array_closure(async));

            AssertSql();
        }

        // TODO: Implement TranslatePrimitiveCollection.
        public override async Task Contains_with_local_tuple_array_closure(bool async)
        {
            // await AssertTranslationFailed(() => base.Contains_with_local_tuple_array_closure(async));

            await Assert.ThrowsAsync<InvalidOperationException>(() => base.Contains_with_local_tuple_array_closure(async));
        }

        public override async Task Contains_with_local_enumerable_inline(bool async)
        {
            // Issue #31776
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                    await base.Contains_with_local_enumerable_inline(async));

            AssertSql();
        }

        public override async Task Contains_with_local_enumerable_inline_closure_mix(bool async)
        {
            // Issue #31776
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () =>
                    await base.Contains_with_local_enumerable_inline_closure_mix(async));

            AssertSql();
        }

        public override Task Contains_inside_Average_without_GroupBy(bool async)
            => AssertAverage(
                async,
                ss => ss.Set<OrderDetail>().Where(od => od.OrderID == 10250),
                selector: od => od.UnitPrice,
                asserter: (a, b) => Assert.Equal(a, b, 6)); // added floating point precision tolerance

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
