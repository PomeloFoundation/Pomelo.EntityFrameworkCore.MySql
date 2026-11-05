using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.Query;
using Xunit.Abstractions;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests
{
    internal class NorthwindQueryTaggingQueryMySqlTest : NorthwindQueryTaggingQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindQueryTaggingQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }
    }
}
