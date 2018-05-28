using EFCore.MySql.UpstreamFunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class NullKeysMySqlTest : NullKeysTestBase<NullKeysMySqlTest.NullKeysMySqlFixture>
    {
        public NullKeysMySqlTest(NullKeysMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class NullKeysMySqlFixture : NullKeysFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
        }
    }
}
