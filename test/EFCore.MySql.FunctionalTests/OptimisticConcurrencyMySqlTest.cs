using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests
{
    public class OptimisticConcurrencyMySqlTest : OptimisticConcurrencyRelationalTestBase<F1MySqlFixture, byte[]>
    {
        public OptimisticConcurrencyMySqlTest(F1MySqlFixture fixture)
            : base(fixture)
        {
        }

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());

        [ConditionalFact(Skip = "#588")]
        public override Task Updating_then_deleting_the_same_entity_results_in_DbUpdateConcurrencyException_which_can_be_resolved_with_store_values()
        {
            return base.Updating_then_deleting_the_same_entity_results_in_DbUpdateConcurrencyException_which_can_be_resolved_with_store_values();
        }

        [ConditionalFact(Skip = "#588")]
        public override Task Simple_concurrency_exception_can_be_resolved_with_store_values()
        {
            return base.Simple_concurrency_exception_can_be_resolved_with_store_values();
        }
    }
}
