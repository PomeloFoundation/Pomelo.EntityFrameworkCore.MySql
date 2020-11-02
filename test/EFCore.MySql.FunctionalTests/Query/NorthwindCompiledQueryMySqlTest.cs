using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindCompiledQueryMySqlTest : NorthwindCompiledQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindCompiledQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            fixture.TestSqlLoggerFactory.Clear();
            //fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
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

        public override void MakeBinary_does_not_throw_for_unsupported_operator()
        {
            Assert.Equal(
                CoreStrings.TranslationFailed("DbSet<Customer>()    .Where(c => c.CustomerID == (string)(__parameters[0]))"),
                Assert.Throws<InvalidOperationException>(
                    () => base.MakeBinary_does_not_throw_for_unsupported_operator()).Message.Replace("\r", "").Replace("\n", ""));
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
