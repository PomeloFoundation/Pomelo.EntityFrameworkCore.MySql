using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class TransactionMySqlTest : TransactionTestBase<TransactionMySqlTest.TransactionMySqlFixture>
    {
        public TransactionMySqlTest(TransactionMySqlFixture fixture)
            : base(fixture)
        {
        }

        protected override bool SnapshotSupported => false;
        protected override bool AmbientTransactionsSupported => false;

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

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder) => base.AddOptions(builder);
        }
    }
}
