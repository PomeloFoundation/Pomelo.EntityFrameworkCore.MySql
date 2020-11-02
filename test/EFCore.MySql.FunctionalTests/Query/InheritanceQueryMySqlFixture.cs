using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.InheritanceModel;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class InheritanceQueryMySqlFixture : InheritanceQueryRelationalFixture
    {
        protected override ITestStoreFactory TestStoreFactory
            => MySqlTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

#pragma warning disable CS0618 // Type or member is obsolete
            modelBuilder.Entity<AnimalQuery>()
                .HasNoKey()
                .ToQuery(
                    () => context.Set<AnimalQuery>()
                        .FromSqlRaw(@"SELECT * FROM `Animals`"));
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
