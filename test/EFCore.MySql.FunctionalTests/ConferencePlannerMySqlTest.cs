using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

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
        }
    }
}
