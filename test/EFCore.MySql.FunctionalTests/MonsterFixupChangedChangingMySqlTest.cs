using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class MonsterFixupChangedChangingMySqlTest : MonsterFixupTestBase<MonsterFixupChangedChangingMySqlTest.MonsterFixupChangedChangingMySqlFixture>
    {
        public MonsterFixupChangedChangingMySqlTest(MonsterFixupChangedChangingMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class MonsterFixupChangedChangingMySqlFixture : MonsterFixupChangedChangingFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            protected override void OnModelCreating<TMessage, TProduct, TProductPhoto, TProductReview, TComputerDetail, TDimensions>(
                ModelBuilder builder)
            {
                base.OnModelCreating<TMessage, TProduct, TProductPhoto, TProductReview, TComputerDetail, TDimensions>(builder);

                builder.Entity<TMessage>().HasKey(e => e.MessageId);
                builder.Entity<TProductPhoto>().HasKey(e => e.PhotoId);
                builder.Entity<TProductReview>().HasKey(e => e.ReviewId);
            }
        }
    }
}
