using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class IncludeMySqlTest : IncludeTestBase<IncludeMySqlFixture>
    {
        public IncludeMySqlTest(IncludeMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory]
        [InlineData(false)]
        [InlineData(true)]
        public override void Include_duplicate_collection_result_operator(bool useString)
        {
            using (var context = CreateContext())
            {
                var customers
                    = useString
                        ? (from c1 in context.Set<Customer>()
                               .Include("Orders")
                               .OrderBy(c => c.CustomerID)
                               .Take(2)
                           from c2 in context.Set<Customer>()
                               .Include("Orders")
                               .OrderBy(c => c.CustomerID)
                               .Skip(2)
                               .Take(2)
                           select new
                           {
                               c1,
                               c2
                           })
                        .OrderBy(p => p.c1) // <-- needs explicit sorting
                        .ThenBy(p => p.c2) // <-- needs explicit sorting
                        .Take(1)
                        .ToList()
                        : (from c1 in context.Set<Customer>()
                               .Include(c => c.Orders)
                               .OrderBy(c => c.CustomerID)
                               .Take(2)
                           from c2 in context.Set<Customer>()
                               .Include(c => c.Orders)
                               .OrderBy(c => c.CustomerID)
                               .Skip(2)
                               .Take(2)
                           select new
                           {
                               c1,
                               c2
                           })
                        .OrderBy(p => p.c1) // <-- needs explicit sorting
                        .ThenBy(p => p.c2) // <-- needs explicit sorting
                        .Take(1)
                        .ToList();

                Assert.Single(customers);
                Assert.Equal(6, customers.SelectMany(c => c.c1.Orders).Count());
                Assert.True(customers.SelectMany(c => c.c1.Orders).All(o => o.Customer != null));
                Assert.Equal(7, customers.SelectMany(c => c.c2.Orders).Count());
                Assert.True(customers.SelectMany(c => c.c2.Orders).All(o => o.Customer != null));
                Assert.Equal(15, context.ChangeTracker.Entries().Count());

                foreach (var customer in customers.Select(e => e.c1))
                {
                    CheckIsLoaded(
                        context,
                        customer,
                        ordersLoaded: true,
                        orderDetailsLoaded: false,
                        productLoaded: false);
                }

                foreach (var customer in customers.Select(e => e.c2))
                {
                    CheckIsLoaded(
                        context,
                        customer,
                        ordersLoaded: true,
                        orderDetailsLoaded: false,
                        productLoaded: false);
                }
            }
        }

        private static void CheckIsLoaded(
            NorthwindContext context,
            Customer customer,
            bool ordersLoaded,
            bool orderDetailsLoaded,
            bool productLoaded)
        {
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            Assert.Equal(ordersLoaded, context.Entry(customer).Collection(e => e.Orders).IsLoaded);
            if (customer.Orders != null)
            {
                foreach (var order in customer.Orders)
                {
                    Assert.Equal(ordersLoaded, context.Entry(order).Reference(e => e.Customer).IsLoaded);

                    Assert.Equal(orderDetailsLoaded, context.Entry(order).Collection(e => e.OrderDetails).IsLoaded);
                    if (order.OrderDetails != null)
                    {
                        foreach (var orderDetail in order.OrderDetails)
                        {
                            Assert.Equal(orderDetailsLoaded, context.Entry(orderDetail).Reference(e => e.Order).IsLoaded);

                            Assert.Equal(productLoaded, context.Entry(orderDetail).Reference(e => e.Product).IsLoaded);
                            if (orderDetail.Product != null)
                            {
                                Assert.False(context.Entry(orderDetail.Product).Collection(e => e.OrderDetails).IsLoaded);
                            }
                        }
                    }
                }
            }
        }
    }
}
