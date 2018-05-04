using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit.Abstractions;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class DbFunctionsMySqlTest : DbFunctionsTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public DbFunctionsMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
        }
    }
}
