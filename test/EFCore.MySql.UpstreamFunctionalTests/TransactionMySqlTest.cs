using EFCore.MySql.UpstreamFunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests
{
    public class TransactionMySqlTest : TransactionTestBase<TransactionMySqlTest.TransactionMySqlFixture>
    {
        public TransactionMySqlTest(TransactionMySqlFixture fixture)
            : base(fixture)
        {
        }

        protected override bool SnapshotSupported => false;

        protected override DbContext CreateContextWithConnectionString()
        {
            var options = Fixture.AddOptions(
                    new DbContextOptionsBuilder()
                        .UseMySql(TestStore.ConnectionString))
                .UseInternalServiceProvider(Fixture.ServiceProvider);

            return new DbContext(options.Options);
        }

        public class TransactionMySqlFixture : TransactionFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            public override void Reseed()
            {
                using (var context = CreateContext())
                {
                    context.Set<TransactionCustomer>().RemoveRange(context.Set<TransactionCustomer>());
                    context.SaveChanges();

                    Seed(context);
                }
            }

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                return base.AddOptions(builder)
                    .ConfigureWarnings(w
                        => w.Log(RelationalEventId.QueryClientEvaluationWarning)
                            .Log(CoreEventId.FirstWithoutOrderByAndFilterWarning));
            }
        }
    }
}
