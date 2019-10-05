using System;
using System.Threading.Tasks;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class AsyncSimpleQueryMySqlTest
    {
        [ConditionalFact]
        public async Task Single_Predicate_Cancellation()
        {
            await Assert.ThrowsAsync<OperationCanceledException>(
                async () =>
                    await Single_Predicate_Cancellation_test(Fixture.TestSqlLoggerFactory.CancelQuery()));
        }
    }
}
