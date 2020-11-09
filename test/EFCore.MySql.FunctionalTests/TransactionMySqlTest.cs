using System;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;
using Pomelo.EntityFrameworkCore.MySql.Tests;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class TransactionMySqlTest : TransactionTestBase<TransactionMySqlTest.TransactionMySqlFixture>
    {
        public TransactionMySqlTest(TransactionMySqlFixture fixture)
            : base(fixture)
        {
        }

        protected override bool SnapshotSupported => false;
        protected override bool AmbientTransactionsSupported => true;
        protected override bool DirtyReadsOccur => false;

        protected override DbContext CreateContextWithConnectionString()
        {
            var options = Fixture.AddOptions(
                    new DbContextOptionsBuilder()
                        .UseMySql(
                            TestStore.ConnectionString,
                            AppConfig.ServerVersion,
                            b => MySqlTestStore.AddOptions(b).ExecutionStrategy(c => new MySqlExecutionStrategy(c))))
                .UseInternalServiceProvider(Fixture.ServiceProvider);

            return new DbContext(options.Options);
        }

        public class TransactionMySqlFixture : TransactionFixtureBase
        {
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

            public override void Reseed()
            {
                using var context = CreateContext();
                context.Set<TransactionCustomer>().RemoveRange(context.Set<TransactionCustomer>());
                context.Set<TransactionOrder>().RemoveRange(context.Set<TransactionOrder>());
                context.SaveChanges();

                base.Seed(context);
            }

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                new MySqlDbContextOptionsBuilder(
                        base.AddOptions(builder))
                    .ExecutionStrategy(c => new MySqlExecutionStrategy(c));
                return builder;
            }

            // public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            // {
            //     new MySqlDbContextOptionsBuilder(base.AddOptions(builder))
            //         .MaxBatchSize(1);
            //     return builder;
            // }
        }
    }
}
