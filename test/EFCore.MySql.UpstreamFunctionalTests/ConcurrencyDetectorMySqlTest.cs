using EFCore.MySql.UpstreamFunctionalTests.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests
{
    public class ConcurrencyDetectorMySqlTest : ConcurrencyDetectorRelationalTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public ConcurrencyDetectorMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }
    }
}
