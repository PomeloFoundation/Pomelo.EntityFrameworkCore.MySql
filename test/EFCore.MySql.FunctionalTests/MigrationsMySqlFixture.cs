using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class MigrationsMySqlFixture : MigrationsFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory => MySqlConnectionStringTestStoreFactory.Instance;
    }
}
