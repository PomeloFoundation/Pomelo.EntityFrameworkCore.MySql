using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.ConcurrencyModel;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class OptimisticConcurrencyMySqlTest : OptimisticConcurrencyTestBase<F1MySqlFixture>
    {
        public OptimisticConcurrencyMySqlTest(F1MySqlFixture fixture)
            : base(fixture)
        {
        }

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());

        protected override Task ConcurrencyTestAsync(
            Action<F1Context> storeChange, Action<F1Context> clientChange,
            Action<F1Context, DbUpdateException> resolver, Action<F1Context> validator)
        {
            return base.ConcurrencyTestAsync(c =>
            {
                storeChange(c);
                // Need to wait to make CURRENT_TIMESTAMP return different values reliably
                Thread.Sleep(500);
            }, clientChange, resolver, validator);
        }
    }
}
