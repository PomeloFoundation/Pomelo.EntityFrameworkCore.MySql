using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests
{
    public class NotificationEntitiesMySqlTest : NotificationEntitiesTestBase<NotificationEntitiesMySqlTest.NotificationEntitiesMySqlFixture>
    {
        public NotificationEntitiesMySqlTest(NotificationEntitiesMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class NotificationEntitiesMySqlFixture : NotificationEntitiesFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
        }
    }
}
