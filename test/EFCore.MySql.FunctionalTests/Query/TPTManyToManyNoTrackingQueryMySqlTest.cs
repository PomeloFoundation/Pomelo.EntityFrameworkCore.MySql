using Microsoft.EntityFrameworkCore.Query;
using Xunit;
using Microsoft.EntityFrameworkCore.Query.Inheritance;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class TPTManyToManyNoTrackingQueryMySqlTest : TPTManyToManyNoTrackingQueryRelationalTestBase<TPTManyToManyQueryMySqlFixture>
    {
        public TPTManyToManyNoTrackingQueryMySqlTest(TPTManyToManyQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        // Shares its fixture (and so its default store name) with the tracking variant. Without its own store, the
        // non-shared tests of both classes run in parallel against the same database and drop each other's tables.
        protected override string NonSharedStoreName
            => nameof(TPTManyToManyNoTrackingQueryMySqlTest) + "_NonShared";
    }
}
