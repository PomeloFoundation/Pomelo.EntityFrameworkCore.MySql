using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class CompiledQueryMySqlTest : CompiledQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public CompiledQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }
    }
}
