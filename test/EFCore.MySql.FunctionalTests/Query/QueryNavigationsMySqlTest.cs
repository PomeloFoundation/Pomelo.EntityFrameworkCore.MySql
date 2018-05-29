using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class QueryNavigationsMySqlTest : QueryNavigationsTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public QueryNavigationsMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }
    }
}
