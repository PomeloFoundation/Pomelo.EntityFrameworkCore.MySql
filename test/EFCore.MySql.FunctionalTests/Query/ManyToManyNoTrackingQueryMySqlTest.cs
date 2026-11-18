using Microsoft.EntityFrameworkCore.Query;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ManyToManyNoTrackingQueryMySqlTest
        : ManyToManyNoTrackingQueryRelationalTestBase<ManyToManyQueryMySqlFixture>
    {
        public ManyToManyNoTrackingQueryMySqlTest(ManyToManyQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        // Shares its fixture (and so its default store name) with the tracking variant. Without its own store, the
        // non-shared tests of both classes run in parallel against the same database and drop each other's tables.
        protected override string NonSharedStoreName
            => nameof(ManyToManyNoTrackingQueryMySqlTest) + "_NonShared";
    }
}
