using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
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
