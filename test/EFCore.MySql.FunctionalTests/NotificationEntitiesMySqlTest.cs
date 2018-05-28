using EFCore.MySql.UpstreamFunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests
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
