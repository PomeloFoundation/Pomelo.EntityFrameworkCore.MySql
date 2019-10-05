using System;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class TransactionMySqlTest : TransactionTestBase<TransactionMySqlTest.TransactionMySqlFixture>, IDisposable
    {
        public TransactionMySqlTest(TransactionMySqlFixture fixture)
            : base(fixture)
        {
            TestMySqlRetryingExecutionStrategy.Suspended = true;
        }

        public void Dispose()
        {
            TestMySqlRetryingExecutionStrategy.Suspended = false;
        }
        
        protected override bool SnapshotSupported => false;
        protected override bool AmbientTransactionsSupported => true;
        protected override bool DirtyReadsOccur => false;

        protected override DbContext CreateContextWithConnectionString()
        {
            var options = Fixture.AddOptions(
                    new DbContextOptionsBuilder()
                        .UseMySql(TestStore.ConnectionString, MySqlTestStore.AddOptions))
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
                new MySqlDbContextOptionsBuilder(base.AddOptions(builder))
                    .MaxBatchSize(1);
                return builder;
            }
        }
    }
}
