using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.Design.Internal;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests
{
    public class DesignTimeMySqlTest : DesignTimeTestBase<DesignTimeMySqlTest.DesignTimeMySqlFixture>
    {
        public DesignTimeMySqlTest(DesignTimeMySqlFixture fixture)
            : base(fixture)
        {
        }

        protected override Assembly ProviderAssembly
            => typeof(MySqlDesignTimeServices).Assembly;

        public class DesignTimeMySqlFixture : DesignTimeFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => MySqlTestStoreFactory.Instance;
        }
    }
}
