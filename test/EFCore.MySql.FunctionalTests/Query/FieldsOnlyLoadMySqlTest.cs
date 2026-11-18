using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class FieldsOnlyLoadMySqlTest : FieldsOnlyLoadTestBase<FieldsOnlyLoadMySqlTest.FieldsOnlyLoadMySqlFixture>
    {
        public FieldsOnlyLoadMySqlTest(FieldsOnlyLoadMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class FieldsOnlyLoadMySqlFixture : FieldsOnlyLoadFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => MySqlTestStoreFactory.Instance;
        }
    }
}
