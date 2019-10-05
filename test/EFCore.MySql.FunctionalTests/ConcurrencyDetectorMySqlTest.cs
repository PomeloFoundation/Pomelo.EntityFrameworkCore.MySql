using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class ConcurrencyDetectorMySqlTest : ConcurrencyDetectorRelationalTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public ConcurrencyDetectorMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }
    }
}
