using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Xunit;

// ReSharper disable InconsistentNaming
#pragma warning disable 1998
namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class AsyncSimpleQueryMySqlTest : AsyncSimpleQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public AsyncSimpleQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }

        public override async Task Query_backed_by_database_view()
        {
            // Not present on SQLite
        }

        public async Task Skip_when_no_order_by()
        {
            await Assert.ThrowsAsync<Exception>(async () => await AssertQuery<Customer>(cs => cs.Skip(5).Take(10)));
        }

        [Fact]
        public async Task Single_Predicate_Cancellation()
        {
            await Assert.ThrowsAsync<TaskCanceledException>(
                async () =>
                    await Single_Predicate_Cancellation_test(Fixture.TestSqlLoggerFactory.CancelQuery()));
        }

        [Fact]
        public override async Task String_Contains_Literal()
        {
            await AssertQuery<Customer>(
                cs => cs.Where(c => c.ContactName.Contains("M")), // case-insensitive
                cs => cs.Where(c => c.ContactName.Contains("M") || c.ContactName.Contains("m")), // case-sensitive
                entryCount: 34);
        }

        [Fact]
        public override async Task String_Contains_MethodCall()
        {
            await AssertQuery<Customer>(
                cs => cs.Where(c => c.ContactName.Contains(LocalMethod1())), // case-insensitive
                cs => cs.Where(c => c.ContactName.Contains(LocalMethod1().ToLower()) || c.ContactName.Contains(LocalMethod1().ToUpper())), // case-sensitive
                entryCount: 34);
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task Average_with_binary_expression()
        {
            return base.Average_with_binary_expression();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task Average_with_arg()
        {
            return base.Average_with_arg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task Average_with_no_arg()
        {
            return base.Average_with_no_arg();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task Average_with_arg_expression()
        {
            return base.Average_with_arg_expression();
        }
    }
}
