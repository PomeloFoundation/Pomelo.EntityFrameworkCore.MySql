using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.ConferencePlanner;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class ConferencePlannerMySqlTest : ConferencePlannerTestBase<ConferencePlannerMySqlTest.ConferencePlannerMySqlFixture>
    {
        public ConferencePlannerMySqlTest(ConferencePlannerMySqlFixture fixture)
            : base(fixture)
        {
        }

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());

        public class ConferencePlannerMySqlFixture : ConferencePlannerFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                base.OnModelCreating(modelBuilder, context);

                // In MySQL 5.6 and lower, unique indices have a smaller max. key size as in later version.
                // This can lead to the following exception being thrown:
                //     Specified key was too long; max key length is 767 bytes
                if (!AppConfig.ServerVersion.Supports.LargerKeyLength)
                {
                    modelBuilder.Entity<Attendee>(entity =>
                    {
                        entity.Property(e => e.UserName)
                            .HasMaxLength(AppConfig.ServerVersion.MaxKeyLength / 4);

                        entity.HasAlternateKey(e => e.UserName);
                    });
                }

                // Some of the data requires a UTF-8 character set.
                modelBuilder.Entity<Speaker>()
                    .Property(e => e.Name)
                    .HasCharSet(CharSet.Utf8Mb4);
                modelBuilder.Entity<Session>()
                    .Property(e => e.Title)
                    .HasCharSet(CharSet.Utf8Mb4);
            }
        }
    }
}
