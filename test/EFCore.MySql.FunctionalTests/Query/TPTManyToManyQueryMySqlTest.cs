using Microsoft.EntityFrameworkCore.Query;
using Xunit;
using Microsoft.EntityFrameworkCore.Query.Inheritance;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class TPTManyToManyQueryMySqlTest : TPTManyToManyQueryRelationalTestBase<TPTManyToManyQueryMySqlFixture>
    {
        public TPTManyToManyQueryMySqlTest(TPTManyToManyQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }
    }
}
