using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class QueryNavigationsMySqlTest : QueryNavigationsTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public QueryNavigationsMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }
    }
}
