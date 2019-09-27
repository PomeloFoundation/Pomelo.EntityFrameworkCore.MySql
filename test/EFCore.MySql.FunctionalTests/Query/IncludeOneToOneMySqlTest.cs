using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class IncludeOneToOneMySqlTest : IncludeOneToOneTestBase<IncludeOneToOneMySqlTest.OneToOneQueryMySqlFixture>
    {
        public IncludeOneToOneMySqlTest(OneToOneQueryMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class OneToOneQueryMySqlFixture : OneToOneQueryFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            public TestSqlLoggerFactory TestSqlLoggerFactory =>
                (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();
        }
    }
}
