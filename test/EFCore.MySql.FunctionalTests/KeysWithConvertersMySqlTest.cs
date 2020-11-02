using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

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
                => builder.UseMySql(b => b.MinBatchSize(1));
        }
    }
}
