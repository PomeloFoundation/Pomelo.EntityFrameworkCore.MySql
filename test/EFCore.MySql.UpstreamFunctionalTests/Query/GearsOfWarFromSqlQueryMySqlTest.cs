using Microsoft.EntityFrameworkCore.Query;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class GearsOfWarFromSqlQueryMySqlTest : GearsOfWarFromSqlQueryTestBase<GearsOfWarQueryMySqlFixture>
    {
        public GearsOfWarFromSqlQueryMySqlTest(GearsOfWarQueryMySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
