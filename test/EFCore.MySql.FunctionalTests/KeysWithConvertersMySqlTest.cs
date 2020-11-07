using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Tests;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
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
