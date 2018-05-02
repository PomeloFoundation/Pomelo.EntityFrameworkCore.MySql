using Microsoft.EntityFrameworkCore.Query;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class WarningsMySqlTest : WarningsTestBase<QueryNoClientEvalMySqlFixture>
    {
        public WarningsMySqlTest(QueryNoClientEvalMySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
