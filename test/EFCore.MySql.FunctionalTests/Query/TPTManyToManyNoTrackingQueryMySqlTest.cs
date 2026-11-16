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
    }
}
