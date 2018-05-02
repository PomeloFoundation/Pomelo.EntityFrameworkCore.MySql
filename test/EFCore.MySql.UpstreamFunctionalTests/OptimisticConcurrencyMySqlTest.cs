using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFCore.MySql.UpstreamFunctionalTests
{
    public class OptimisticConcurrencyMySqlTest : OptimisticConcurrencyTestBase<F1MySqlFixture>
    {
        public OptimisticConcurrencyMySqlTest(F1MySqlFixture fixture)
            : base(fixture)
        {
        }

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());
    }
}
