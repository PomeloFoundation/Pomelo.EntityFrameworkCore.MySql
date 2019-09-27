using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class FunkyDataQueryMySqlTest : FunkyDataQueryTestBase<FunkyDataQueryMySqlTest.FunkyDataQueryMySqlFixture>
    {
        public FunkyDataQueryMySqlTest(FunkyDataQueryMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class FunkyDataQueryMySqlFixture : FunkyDataQueryFixtureBase
        {
            public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();

            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
        }
    }
}
