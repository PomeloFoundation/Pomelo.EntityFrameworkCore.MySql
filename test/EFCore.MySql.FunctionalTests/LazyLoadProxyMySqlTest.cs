using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
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
                => base.AddOptions(builder);
        }
    }
}
