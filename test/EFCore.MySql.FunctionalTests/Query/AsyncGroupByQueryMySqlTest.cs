using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class AsyncGroupByQueryMySqlTest : AsyncGroupByQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        // ReSharper disable once UnusedParameter.Local
        public AsyncGroupByQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalFact(Skip = "Reimplemented in Join_GroupBy_entity_ToList_fixed_with_sorting, due to lack of sorting the results, which can lead to failing tests (false positive).")]
        public override Task Join_GroupBy_entity_ToList()
        {
            return base.Join_GroupBy_entity_ToList();
        }

        [Fact]
        public async Task Join_GroupBy_entity_ToList_fixed_with_sorting()
        {
            //
            // Overridden due to it's base implementations lack of sorting the results,
            // which can lead to failing tests (false positive).
            //

            using (var context = CreateContext())
            {
                var actual = await (from c in context.Customers.OrderBy(c => c.CustomerID).Take(5)
                    join o in context.Orders.OrderBy(o => o.OrderID).Take(50)
                        on c.CustomerID equals o.CustomerID
                    group o by c
                    into grp
                    select new
                    {
                        C = grp.Key,
                        Os = grp.OrderBy(o => o.OrderID).ToList()
                    }).ToListAsync();

                var expected = (from c in Fixture.QueryAsserter.ExpectedData.Set<Customer>()
                        .OrderBy(c => c.CustomerID).Take(5)
                    join o in Fixture.QueryAsserter.ExpectedData.Set<Order>()
                            .OrderBy(o => o.OrderID).Take(50)
                        on c.CustomerID equals o.CustomerID
                    group o by c
                    into grp
                    select new
                    {
                        C = grp.Key,
                        Os = grp.OrderBy(o => o.OrderID).ToList()
                    }).ToList();

                Assert.Equal(expected.Count, actual.Count);

                for (var i = 0; i < expected.Count; i++)
                {
                    Assert.Equal(expected[i].C, actual[i].C);
                    Assert.Equal(expected[i].Os, actual[i].Os);
                }
            }
        }
    }
}
