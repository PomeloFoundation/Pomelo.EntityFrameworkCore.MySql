using Microsoft.EntityFrameworkCore.Query;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class GearsOfWarFromSqlQueryMySqlTest : GearsOfWarFromSqlQueryTestBase<GearsOfWarQueryMySqlFixture>
    {
        public GearsOfWarFromSqlQueryMySqlTest(GearsOfWarQueryMySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
