using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class AsTrackingMySqlTest : AsTrackingTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public AsTrackingMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }
    }
}
