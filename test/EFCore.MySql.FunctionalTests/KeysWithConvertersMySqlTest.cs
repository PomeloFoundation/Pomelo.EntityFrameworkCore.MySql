using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microting.EntityFrameworkCore.MySql.Tests;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests
{
    public class KeysWithConvertersMySqlTest : KeysWithConvertersTestBase<
        KeysWithConvertersMySqlTest.KeysWithConvertersMySqlFixture>
    {
        public KeysWithConvertersMySqlTest(KeysWithConvertersMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class KeysWithConvertersMySqlFixture : KeysWithConvertersFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory
                => MySqlTestStoreFactory.Instance;

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                => builder.UseMySql(AppConfig.ServerVersion, b => b.MinBatchSize(1));
        }
    }
}
