using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class IncludeAsyncMySqlTest : IncludeAsyncTestBase<IncludeMySqlFixture>
    {
        public IncludeAsyncMySqlTest(IncludeMySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
