using EFCore.MySql.UpstreamFunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class OwnedQueryMySqlTest : RelationalOwnedQueryTestBase<OwnedQueryMySqlTest.OwnedQueryMySqlFixture>
    {
        public OwnedQueryMySqlTest(OwnedQueryMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class OwnedQueryMySqlFixture : RelationalOwnedQueryFixture
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
        }
    }
}
