using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class AsyncFromSqlQueryMySqlTest : AsyncFromSqlQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public AsyncFromSqlQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }
    }
}
