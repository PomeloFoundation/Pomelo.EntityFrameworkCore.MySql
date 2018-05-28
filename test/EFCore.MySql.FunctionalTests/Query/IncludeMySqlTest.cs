using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class IncludeMySqlTest : IncludeTestBase<IncludeMySqlFixture>
    {
        public IncludeMySqlTest(IncludeMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }
    }
}
