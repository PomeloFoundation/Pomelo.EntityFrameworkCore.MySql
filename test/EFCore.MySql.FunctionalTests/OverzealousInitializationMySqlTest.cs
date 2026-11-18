using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests
{
    public class OverzealousInitializationMySqlTest
        : OverzealousInitializationTestBase<OverzealousInitializationMySqlTest.OverzealousInitializationMySqlFixture>
    {
        public OverzealousInitializationMySqlTest(OverzealousInitializationMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class OverzealousInitializationMySqlFixture : OverzealousInitializationFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
        }
    }
}
