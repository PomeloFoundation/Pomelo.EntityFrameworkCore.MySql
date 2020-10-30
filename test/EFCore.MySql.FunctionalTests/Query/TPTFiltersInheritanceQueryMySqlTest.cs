using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
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
