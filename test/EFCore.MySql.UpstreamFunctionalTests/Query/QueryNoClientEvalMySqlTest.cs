using Microsoft.EntityFrameworkCore.Query;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class QueryNoClientEvalMySqlTest : QueryNoClientEvalTestBase<QueryNoClientEvalMySqlFixture>
    {
        public QueryNoClientEvalMySqlTest(QueryNoClientEvalMySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
