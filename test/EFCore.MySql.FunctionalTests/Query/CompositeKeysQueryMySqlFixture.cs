using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class CompositeKeysQueryMySqlFixture : CompositeKeysQueryRelationalFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
