using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
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
