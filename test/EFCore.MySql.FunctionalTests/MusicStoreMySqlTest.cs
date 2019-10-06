using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class MusicStoreMySqlTest : MusicStoreTestBase<MusicStoreMySqlTest.MusicStoreMySqlFixture>
    {
        public MusicStoreMySqlTest(MusicStoreMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class MusicStoreMySqlFixture : MusicStoreFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                var optionsBuilder = base.AddOptions(builder);

                // In MySQL 5.6 and lower, unique indices have a smaller max. key size as in later version.
                // This can lead to the following exception being thrown:
                //     Specified key was too long; max key length is 767 bytes
                if (!AppConfig.ServerVersion.SupportsLargerKeyLength)
                {
                    new MySqlDbContextOptionsBuilder(optionsBuilder)
                        .UnicodeCharSet(CharSet.Latin1);
                }
                
                return optionsBuilder;
            }
        }
    }
}
