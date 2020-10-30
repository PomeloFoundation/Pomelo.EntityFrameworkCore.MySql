using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class MySqlFixture : ServiceProviderFixtureBase
    {
        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ServiceProvider.GetRequiredService<ILoggerFactory>();

        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;
    }
}
