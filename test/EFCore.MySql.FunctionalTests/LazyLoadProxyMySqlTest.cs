using EFCore.MySql.UpstreamFunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests
{
    public class LazyLoadProxyMySqlTest : LazyLoadProxyTestBase<LazyLoadProxyMySqlTest.LoadMySqlFixture>
    {
        public LazyLoadProxyMySqlTest(LoadMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class LoadMySqlFixture : LoadFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                => base.AddOptions(builder).ConfigureWarnings(
                    c => c.Log(RelationalEventId.QueryClientEvaluationWarning));
        }
    }
}
