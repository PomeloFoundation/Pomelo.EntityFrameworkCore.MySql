using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class CompiledQueryMySqlTest : CompiledQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public CompiledQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }

        [ConditionalFact]
        public override void Query_with_array_parameter()
        {
            var query = EF.CompileQuery(
                (NorthwindContext context, string[] args)
                    => context.Customers.Where(c => c.CustomerID == args[0]));

            using (var context = CreateContext())
            {
                // We still throw an InvalidOperationException, which is expected, but with a different error message,
                // because of the implemented array support for JSON.
                Assert.Throws<InvalidOperationException>(
                    () => query(context, new[] {"ALFKI"})
                        .First()
                        .CustomerID);
            }

            using (var context = CreateContext())
            {
                // We still throw an InvalidOperationException, which is expected, but with a different error message,
                // because of the implemented array support for JSON.
                Assert.Throws<InvalidOperationException>(
                    () => query(context, new[] {"ANATR"})
                        .First()
                        .CustomerID);
            }
        }

        [ConditionalFact]
        public override async Task Query_with_array_parameter_async()
        {
            var query = EF.CompileAsyncQuery(
                (NorthwindContext context, string[] args)
                    => context.Customers.Where(c => c.CustomerID == args[0]));

            using (var context = CreateContext())
            {
                // We still throw an InvalidOperationException, which is expected, but with a different error message,
                // because of the implemented array support for JSON.
                await Assert.ThrowsAsync<InvalidOperationException>(
                    () => query(context, new[] {"ALFKI"})
                        .ToListAsync());
            }

            using (var context = CreateContext())
            {
                // We still throw an InvalidOperationException, which is expected, but with a different error message,
                // because of the implemented array support for JSON.
                await Assert.ThrowsAsync<InvalidOperationException>(
                    () => query(context, new[] {"ANATR"})
                        .ToListAsync());
            }
        }
    }
}
