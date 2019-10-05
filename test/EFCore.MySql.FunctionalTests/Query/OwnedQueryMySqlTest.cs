using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
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
