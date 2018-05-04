using EFCore.MySql.UpstreamFunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests
{
    public class F1MySqlFixture : F1RelationalFixture
    {
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
    }
}
