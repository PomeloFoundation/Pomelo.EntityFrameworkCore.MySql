using Microsoft.EntityFrameworkCore.Query;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class IncludeAsyncMySqlTest : IncludeAsyncTestBase<IncludeMySqlFixture>
    {
        public IncludeAsyncMySqlTest(IncludeMySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
