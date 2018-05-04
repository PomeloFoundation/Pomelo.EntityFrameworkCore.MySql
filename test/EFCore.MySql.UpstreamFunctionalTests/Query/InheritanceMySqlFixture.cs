using EFCore.MySql.UpstreamFunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class InheritanceMySqlFixture : InheritanceRelationalFixture
    {
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
    }
}
