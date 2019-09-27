using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class WarningsMySqlTest : WarningsTestBase<QueryNoClientEvalMySqlFixture>
    {
        public WarningsMySqlTest(QueryNoClientEvalMySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
