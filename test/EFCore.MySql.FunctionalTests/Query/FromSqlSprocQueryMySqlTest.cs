using System;
using System.Linq;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class FromSqlSprocQueryMySqlTest : FromSqlSprocQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public FromSqlSprocQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            fixture.TestSqlLoggerFactory.Clear();
        }

        public override Task From_sql_queryable_stored_procedure_re_projection(bool async)
        {
            // MySQL does not support SELECT on a stored procedure's result set.
            return Assert.ThrowsAsync<MySqlException>(() => base.From_sql_queryable_stored_procedure_re_projection(async));
        }

        public override Task From_sql_queryable_stored_procedure_composed(bool async)
        {
            // MySQL does not support WHERE on a stored procedure's result set.
            return Assert.ThrowsAsync<MySqlException>(() => base.From_sql_queryable_stored_procedure_composed(async));
        }

        public override Task From_sql_queryable_stored_procedure_with_parameter_composed(bool async)
        {
            // MySQL does not support WHERE on a stored procedure's result set.
            return Assert.ThrowsAsync<MySqlException>(() => base.From_sql_queryable_stored_procedure_with_parameter_composed(async));
        }

        public override Task From_sql_queryable_stored_procedure_take(bool async)
        {
            // MySQL does not support ORDER BY on a stored procedure's result set.
            return Assert.ThrowsAsync<MySqlException>(() => base.From_sql_queryable_stored_procedure_take(async));
        }

        public override Task From_sql_queryable_stored_procedure_min(bool async)
        {
            // MySQL does not support GROUP BY/MIN() on a stored procedure's result set.
            return Assert.ThrowsAsync<MySqlException>(() => base.From_sql_queryable_stored_procedure_min(async));
        }

        public override Task From_sql_queryable_stored_procedure_with_include_throws(bool async)
        {
            // MySQL does not support JOIN on a stored procedure's result set.
            return Assert.ThrowsAsync<MySqlException>(() => base.From_sql_queryable_stored_procedure_with_include_throws(async));
        }

        public override Task From_sql_queryable_with_multiple_stored_procedures(bool async)
        {
            // MySQL does neither support calling multiple stored procedures in a query, nor does it support
            // SELECT on multiple stored procedure result sets.
            return Assert.ThrowsAsync<MySqlException>(() => base.From_sql_queryable_with_multiple_stored_procedures(async));
        }

        public override Task From_sql_queryable_stored_procedure_and_select(bool async)
        {
            // MySQL does not support WHERE on a stored procedure's result set.
            return Assert.ThrowsAsync<MySqlException>(() => base.From_sql_queryable_stored_procedure_and_select(async));
        }

        public override Task From_sql_queryable_select_and_stored_procedure(bool async)
        {
            // MySQL does not support WHERE on a stored procedure's result set.
            return Assert.ThrowsAsync<MySqlException>(() => base.From_sql_queryable_select_and_stored_procedure(async));
        }

        protected override string TenMostExpensiveProductsSproc => "CALL `Ten Most Expensive Products`()";
        protected override string CustomerOrderHistorySproc => "CALL `CustOrderHist` ({0})";

        private static T AssertSqlException<T>(Exception exception)
            where T : Exception
        {
            Assert.IsType<T>(exception);
            return (T)exception;
        }
    }
}
