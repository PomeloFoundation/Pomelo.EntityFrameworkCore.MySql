using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class AsyncFromSqlQueryMySqlTest : AsyncFromSqlQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public AsyncFromSqlQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }

        [ConditionalFact(Skip = "This test fails on connections that use a password, because the password is not saved between sessions. It then breaks all upcoming tests from the same test class.")]
        public override Task Include_does_not_close_user_opened_connection_for_empty_result()
        {
            return base.Include_does_not_close_user_opened_connection_for_empty_result();
        }

        [ConditionalFact(Skip = "This test fails on connections that use a password, because the password is not saved between sessions. It then breaks all upcoming tests from the same test class.")]
        public override Task Include_closed_connection_opened_by_it_when_buffering()
        {
            return base.Include_closed_connection_opened_by_it_when_buffering();
        }
    }
}
