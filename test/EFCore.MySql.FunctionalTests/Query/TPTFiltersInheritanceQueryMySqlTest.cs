using Microsoft.EntityFrameworkCore.Query;
using Xunit;
using Microsoft.EntityFrameworkCore.Query.Inheritance;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class TPTFiltersInheritanceQueryMySqlTest : TPTFiltersInheritanceQueryTestBase<TPTFiltersInheritanceQueryMySqlFixture>
    {
        public TPTFiltersInheritanceQueryMySqlTest(TPTFiltersInheritanceQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }
    }
}
