using EFCore.MySql.UpstreamFunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests
{
    public class MonsterFixupChangedOnlyMySqlTest : MonsterFixupTestBase<MonsterFixupChangedOnlyMySqlTest.MonsterFixupChangedOnlyMySqlFixture>
    {
        public MonsterFixupChangedOnlyMySqlTest(MonsterFixupChangedOnlyMySqlFixture fixture)
            : base(fixture)
        {
        }

        public class MonsterFixupChangedOnlyMySqlFixture : MonsterFixupChangedOnlyFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
                => base.AddOptions(builder).ConfigureWarnings(w => w.Log(RelationalEventId.QueryClientEvaluationWarning));

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
