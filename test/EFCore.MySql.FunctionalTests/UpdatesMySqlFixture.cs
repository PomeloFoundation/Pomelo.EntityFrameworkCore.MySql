using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class UpdatesMySqlFixture : UpdatesRelationalFixture
    {
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
    }
}
