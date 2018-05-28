using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit.Abstractions;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class AsyncQueryNavigationsMySqlTest : AsyncQueryNavigationsTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public AsyncQueryNavigationsMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }
    }
}
