using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class AsTrackingMySqlTest : AsTrackingTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public AsTrackingMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }
    }
}
