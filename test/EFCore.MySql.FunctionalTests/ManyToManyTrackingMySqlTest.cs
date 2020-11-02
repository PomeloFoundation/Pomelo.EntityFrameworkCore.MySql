namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class ManyToManyTrackingMySqlTest
        : ManyToManyTrackingMySqlTestBase<ManyToManyTrackingMySqlTest.ManyToManyTrackingMySqlFixture>
    {
        public ManyToManyTrackingMySqlTest(ManyToManyTrackingMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class ManyToManyTrackingMySqlFixture : ManyToManyTrackingMySqlFixtureBase
        {
        }
    }
}
