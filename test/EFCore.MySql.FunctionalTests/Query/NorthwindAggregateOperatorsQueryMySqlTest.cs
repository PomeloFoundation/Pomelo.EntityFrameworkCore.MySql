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

        public override Task Average_over_max_subquery_is_client_eval(bool async)
            => AssertAverage(
                async,
                ss => ss.Set<Customer>().OrderBy(c => c.CustomerID).Take(3),
                selector: c => (decimal)c.Orders.Average(o => 5 + o.OrderDetails.Max(od => od.ProductID)),
                asserter: (a, b) => Assert.Equal(a, b, 12)); // added flouting point precision tolerance

        public override Task Average_over_nested_subquery_is_client_eval(bool async)
            => AssertAverage(
                async,
                ss => ss.Set<Customer>().OrderBy(c => c.CustomerID).Take(3),
                selector: c => (decimal)c.Orders.Average(o => 5 + o.OrderDetails.Average(od => od.ProductID)),
                asserter: (a, b) => Assert.Equal(a, b, 12)); // added flouting point precision tolerance

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

        protected override bool CanExecuteQueryString
            => true;

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();
    }
}
