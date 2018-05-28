using Microsoft.EntityFrameworkCore.Query;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class GearsOfWarQueryMySqlTest : GearsOfWarQueryTestBase<GearsOfWarQueryMySqlFixture>
    {
        public GearsOfWarQueryMySqlTest(GearsOfWarQueryMySqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
