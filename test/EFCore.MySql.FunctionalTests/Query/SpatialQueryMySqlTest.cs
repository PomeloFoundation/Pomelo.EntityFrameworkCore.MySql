using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    // Made internal to skip all tests.
    internal class SpatialQueryMySqlTest : SpatialQueryTestBase<SpatialQueryMySqlFixture>
    {
        public SpatialQueryMySqlTest(SpatialQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
