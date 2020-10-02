using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class FiltersInheritanceQueryMySqlTest : FiltersInheritanceQueryTestBase<FiltersInheritanceQueryMySqlFixture>
    {
        public FiltersInheritanceQueryMySqlTest(FiltersInheritanceQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }
    }
}
