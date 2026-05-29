using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class TPTInheritanceQueryMySqlTest : TPTInheritanceQueryTestBase<TPTInheritanceQueryMySqlFixture>
    {
        public TPTInheritanceQueryMySqlTest(
            TPTInheritanceQueryMySqlFixture fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }
    }
}
