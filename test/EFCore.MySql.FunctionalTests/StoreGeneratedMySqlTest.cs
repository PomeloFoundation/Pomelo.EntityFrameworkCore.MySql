using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class StoreGeneratedMySqlTest : StoreGeneratedTestBase<StoreGeneratedMySqlTest.StoreGeneratedMySqlFixture>
    {
        public StoreGeneratedMySqlTest(StoreGeneratedMySqlFixture fixture)
            : base(fixture)
        {
        }

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());

        public class StoreGeneratedMySqlFixture : StoreGeneratedFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
            {
                modelBuilder.Entity<Gumball>(
                    b =>
                    {
                        b.Property(e => e.Identity).HasMaxLength(1000).HasDefaultValue("Banana Joe");
                        b.Property(e => e.IdentityReadOnlyBeforeSave).HasMaxLength(1000)
                            .HasDefaultValue("Doughnut Sheriff");
                        b.Property(e => e.IdentityReadOnlyAfterSave).HasMaxLength(1000).HasDefaultValue("Anton");
                        b.Property(e => e.AlwaysIdentity).HasMaxLength(1000).HasDefaultValue("Banana Joe");
                        b.Property(e => e.AlwaysIdentityReadOnlyBeforeSave).HasMaxLength(1000)
                            .HasDefaultValue("Doughnut Sheriff");
                        b.Property(e => e.AlwaysIdentityReadOnlyAfterSave).HasMaxLength(1000).HasDefaultValue("Anton");
                        b.Property(e => e.Computed).HasMaxLength(1000).HasDefaultValue("Alan");
                        b.Property(e => e.ComputedReadOnlyBeforeSave).HasMaxLength(1000).HasDefaultValue("Carmen");
                        b.Property(e => e.ComputedReadOnlyAfterSave).HasMaxLength(1000).HasDefaultValue("Tina Rex");
                        b.Property(e => e.AlwaysComputed).HasMaxLength(1000).HasDefaultValue("Alan");
                        b.Property(e => e.AlwaysComputedReadOnlyBeforeSave).HasMaxLength(1000).HasDefaultValue("Carmen");
                        b.Property(e => e.AlwaysComputedReadOnlyAfterSave).HasMaxLength(1000).HasDefaultValue("Tina Rex");
                    });

                modelBuilder.Entity<Anais>(
                    b =>
                    {
                        b.Property(e => e.OnAdd).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddUseBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddIgnoreBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddThrowBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddUseBeforeIgnoreAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddIgnoreBeforeIgnoreAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddThrowBeforeIgnoreAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddUseBeforeThrowAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddIgnoreBeforeThrowAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddThrowBeforeThrowAfter).HasMaxLength(500).HasDefaultValue("Rabbit");

                        b.Property(e => e.OnAddOrUpdate).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateUseBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateIgnoreBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateThrowBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateUseBeforeIgnoreAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateIgnoreBeforeIgnoreAfter).HasMaxLength(500)
                            .HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateThrowBeforeIgnoreAfter).HasMaxLength(500)
                            .HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateUseBeforeThrowAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateIgnoreBeforeThrowAfter).HasMaxLength(500)
                            .HasDefaultValue("Rabbit");
                        b.Property(e => e.OnAddOrUpdateThrowBeforeThrowAfter).HasMaxLength(500)
                            .HasDefaultValue("Rabbit");

                        b.Property(e => e.OnUpdate).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateUseBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateIgnoreBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateThrowBeforeUseAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateUseBeforeIgnoreAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateIgnoreBeforeIgnoreAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateThrowBeforeIgnoreAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateUseBeforeThrowAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateIgnoreBeforeThrowAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                        b.Property(e => e.OnUpdateThrowBeforeThrowAfter).HasMaxLength(500).HasDefaultValue("Rabbit");
                    });

                base.OnModelCreating(modelBuilder, context);
            }
        }
    }
}
