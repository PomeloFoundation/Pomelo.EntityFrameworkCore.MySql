using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

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

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                MySqlTestHelpers.Instance.EnsureSufficientKeySpace(modelBuilder.Model, TestStore);
            }
        }
    }
}
