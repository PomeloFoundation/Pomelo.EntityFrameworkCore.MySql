using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests;

public abstract class GraphUpdatesMySqlTestBase<TFixture> : GraphUpdatesTestBase<TFixture>
    where TFixture : GraphUpdatesMySqlTestBase<TFixture>.GraphUpdatesMySqlFixtureBase, new()
{
    protected GraphUpdatesMySqlTestBase(TFixture fixture)
        : base(fixture)
    {
    }

    protected override IQueryable<Root> ModifyQueryRoot(IQueryable<Root> query)
        => query.AsSplitQuery();

    protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
        => facade.UseTransaction(transaction.GetDbTransaction());

    public abstract class GraphUpdatesMySqlFixtureBase : GraphUpdatesFixtureBase
    {
        public TestSqlLoggerFactory TestSqlLoggerFactory
            => (TestSqlLoggerFactory)ListLoggerFactory;

        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            modelBuilder.Entity<AccessState>(
                b =>
                {
                    b.Property(e => e.AccessStateId).ValueGeneratedNever();
                    b.HasData(new AccessState {AccessStateId = 1});
                });

            modelBuilder.Entity<Cruiser>(
                b =>
                {
                    b.Property(e => e.IdUserState).HasDefaultValue(1);
                    b.HasOne(e => e.UserState).WithMany(e => e.Users).HasForeignKey(e => e.IdUserState);
                });

            modelBuilder.Entity<SomethingOfCategoryA>().Property<int>("CategoryId").HasDefaultValue(1);
            modelBuilder.Entity<SomethingOfCategoryB>().Property(e => e.CategoryId).HasDefaultValue(2);
        }
    }
}
