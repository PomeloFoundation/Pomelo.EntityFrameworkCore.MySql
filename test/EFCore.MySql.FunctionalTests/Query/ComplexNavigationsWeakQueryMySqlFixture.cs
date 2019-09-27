using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ComplexNavigationsWeakQueryMySqlFixture : ComplexNavigationsWeakQueryRelationalFixtureBase
    {
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
    }
}
