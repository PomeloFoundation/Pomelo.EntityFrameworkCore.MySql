using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests
{
    public class CompositeKeyEndToEndMySqlTest : CompositeKeyEndToEndTestBase<CompositeKeyEndToEndMySqlTest.CompositeKeyEndToEndMySqlFixture>
    {
        public CompositeKeyEndToEndMySqlTest(CompositeKeyEndToEndMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class CompositeKeyEndToEndMySqlFixture : CompositeKeyEndToEndFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
        }
    }
}
