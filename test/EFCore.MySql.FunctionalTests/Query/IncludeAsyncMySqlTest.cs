using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class IncludeAsyncMySqlTest : IncludeAsyncTestBase<IncludeMySqlFixture>
    {
        public IncludeAsyncMySqlTest(IncludeMySqlFixture fixture)
            : base(fixture)
        {
        }

        [ConditionalFact]
        public override async Task Include_duplicate_collection_result_operator()
        {
            using (var context = CreateContext())
            {
                var customers
                    = await (from c1 in context.Set<Customer>()
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
                        .ToListAsync();

                Assert.Single(customers);
                Assert.Equal(6, customers.SelectMany(c => c.c1.Orders).Count());
                Assert.True(customers.SelectMany(c => c.c1.Orders).All(o => o.Customer != null));
                Assert.Equal(7, customers.SelectMany(c => c.c2.Orders).Count());
                Assert.True(customers.SelectMany(c => c.c2.Orders).All(o => o.Customer != null));
                Assert.Equal(15, context.ChangeTracker.Entries().Count());
            }
        }
    }
}
