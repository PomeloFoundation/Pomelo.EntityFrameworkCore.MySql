using Microsoft.EntityFrameworkCore.Query;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class GearsOfWarFromSqlQueryMySqlTest : GearsOfWarFromSqlQueryTestBase<GearsOfWarQueryMySqlFixture>
    {
        public GearsOfWarFromSqlQueryMySqlTest(GearsOfWarQueryMySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
