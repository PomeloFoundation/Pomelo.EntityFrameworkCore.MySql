using EFCore.MySql.UpstreamFunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class AsyncFromSqlSprocQueryMySqlTest : AsyncFromSqlSprocQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public AsyncFromSqlSprocQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }

        protected override string TenMostExpensiveProductsSproc => "CALL `Ten Most Expensive Products`()";

        protected override string CustomerOrderHistorySproc => "CALL `CustOrderHist` ({0})";
    }
}
