using System.Data.Common;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySql.Data.MySqlClient;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class SqlExecutorMySqlTest : SqlExecutorTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public SqlExecutorMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }

        protected override DbParameter CreateDbParameter(string name, object value)
            => new MySqlParameter
            {
                ParameterName = name,
                Value = value
            };

        protected override string TenMostExpensiveProductsSproc => @"SELECT * FROM ""Ten Most Expensive Products""()";

        protected override string CustomerOrderHistorySproc => @"SELECT * FROM ""CustOrderHist""(@CustomerID)";

        protected override string CustomerOrderHistoryWithGeneratedParameterSproc => @"SELECT * FROM ""CustOrderHist""({0})";
    }
}
