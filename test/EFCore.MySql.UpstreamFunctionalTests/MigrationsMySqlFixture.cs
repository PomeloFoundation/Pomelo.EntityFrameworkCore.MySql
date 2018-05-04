using EFCore.MySql.UpstreamFunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests
{
    public class MigrationsMySqlFixture : MigrationsFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory => MySqlConnectionStringTestStoreFactory.Instance;
    }
}
