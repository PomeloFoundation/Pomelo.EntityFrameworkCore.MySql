using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySqlConnector;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class SqlExecutorMySqlTest : SqlExecutorTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public SqlExecutorMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }

        protected virtual int DefaultStoredProcedureResult
            => 0;

        protected virtual int DefaultSqlResult
            => -1;

        public override async Task Executes_stored_procedure(bool async)
        {
            using var context = CreateContext();
            Assert.Equal(DefaultStoredProcedureResult, context.Database.ExecuteSqlRaw(TenMostExpensiveProductsSproc));
        }

        public override async Task Executes_stored_procedure_with_parameter(bool async)
        {
            using var context = CreateContext();
            var parameter = CreateDbParameter("@CustomerID", "ALFKI");

            Assert.Equal(DefaultStoredProcedureResult, context.Database.ExecuteSqlRaw(CustomerOrderHistorySproc, parameter));
        }

        public override async Task Executes_stored_procedure_with_generated_parameter(bool async)
        {
            using var context = CreateContext();
            Assert.Equal(DefaultStoredProcedureResult, context.Database.ExecuteSqlRaw(CustomerOrderHistoryWithGeneratedParameterSproc, "ALFKI"));
        }

        public override async Task Query_with_parameters_interpolated_2(bool async)
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using var context = CreateContext();
            var actual = context.Database
                .ExecuteSql(
                    $@"SELECT COUNT(*) FROM `Customers` WHERE `City` = {city} AND `ContactTitle` = {contactTitle}");

            Assert.Equal(DefaultSqlResult, actual);
        }

        public override async Task Query_with_DbParameters_interpolated_2(bool async)
        {
            var city = CreateDbParameter("city", "London");
            var contactTitle = CreateDbParameter("contactTitle", "Sales Representative");

            using var context = CreateContext();
            var actual = context.Database
                .ExecuteSql(
                    $@"SELECT COUNT(*) FROM `Customers` WHERE `City` = {city} AND `ContactTitle` = {contactTitle}");

            Assert.Equal(DefaultSqlResult, actual);
        }

        public async Task Executes_stored_procedure_with_parameter_async()
        {
            using var context = CreateContext();
            var parameter = CreateDbParameter("@CustomerID", "ALFKI");

            Assert.Equal(DefaultStoredProcedureResult, await context.Database.ExecuteSqlRawAsync(CustomerOrderHistorySproc, parameter));
        }

        public async Task Executes_stored_procedure_with_generated_parameter_async()
        {
            using var context = CreateContext();
            Assert.Equal(DefaultStoredProcedureResult, await context.Database.ExecuteSqlRawAsync(CustomerOrderHistoryWithGeneratedParameterSproc, "ALFKI"));
        }

        public async Task Query_with_parameters_interpolated_async_2()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using var context = CreateContext();
            var actual = await context.Database
                .ExecuteSqlAsync(
                    $@"SELECT COUNT(*) FROM `Customers` WHERE `City` = {city} AND `ContactTitle` = {contactTitle}");

            Assert.Equal(DefaultSqlResult, actual);
        }

        public override async Task Query_with_parameters(bool async)
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using (var context = CreateContext())
            {
                var actual = context.Database
                    .ExecuteSqlRaw(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = {0} AND `ContactTitle` = {1}", city, contactTitle);

                Assert.Equal(DefaultSqlResult, actual);
            }
        }

        public override async Task Query_with_dbParameter_with_name(bool async)
        {
            var city = CreateDbParameter("@city", "London");

            using (var context = CreateContext())
            {
                var actual = context.Database
                    .ExecuteSqlRaw(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = @city", city);

                Assert.Equal(DefaultSqlResult, actual);
            }
        }

        public override async Task Query_with_positional_dbParameter_with_name(bool async)
        {
            var city = CreateDbParameter("@city", "London");

            using (var context = CreateContext())
            {
                var actual = context.Database
                    .ExecuteSqlRaw(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = {0}", city);

                Assert.Equal(DefaultSqlResult, actual);
            }
        }

        public override async Task Query_with_positional_dbParameter_without_name(bool async)
        {
            var city = CreateDbParameter(name: null, value: "London");

            using (var context = CreateContext())
            {
                var actual = context.Database
                    .ExecuteSqlRaw(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = {0}", city);

                Assert.Equal(DefaultSqlResult, actual);
            }
        }

        public override async Task Query_with_dbParameters_mixed(bool async)
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            var cityParameter = CreateDbParameter("@city", city);
            var contactTitleParameter = CreateDbParameter("@contactTitle", contactTitle);

            using (var context = CreateContext())
            {
                var actual = context.Database
                    .ExecuteSqlRaw(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = {0} AND `ContactTitle` = @contactTitle", city, contactTitleParameter);

                Assert.Equal(DefaultSqlResult, actual);

                actual = context.Database
                    .ExecuteSqlRaw(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = @city AND `ContactTitle` = {1}", cityParameter, contactTitle);

                Assert.Equal(DefaultSqlResult, actual);
            }
        }

        public override async Task Query_with_parameters_interpolated(bool async)
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using (var context = CreateContext())
            {
                var actual = context.Database
                    .ExecuteSqlInterpolated(
                        $@"SELECT COUNT(*) FROM `Customers` WHERE `City` = {city} AND `ContactTitle` = {contactTitle}");

                Assert.Equal(DefaultSqlResult, actual);
            }
        }

        public async Task Query_with_parameters_async()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using (var context = CreateContext())
            {
                var actual = await context.Database
                    .ExecuteSqlRawAsync(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = {0} AND `ContactTitle` = {1}", city, contactTitle);

                Assert.Equal(DefaultSqlResult, actual);
            }
        }

        public async Task Query_with_parameters_interpolated_async()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using (var context = CreateContext())
            {
                var actual = await context.Database
                    .ExecuteSqlInterpolatedAsync(
                        $@"SELECT COUNT(*) FROM `Customers` WHERE `City` = {city} AND `ContactTitle` = {contactTitle}");

                Assert.Equal(DefaultSqlResult, actual);
            }
        }

        public override async Task Query_with_DbParameters_interpolated(bool async)
        {
            var city = CreateDbParameter("city", "London");
            var contactTitle = CreateDbParameter("contactTitle", "Sales Representative");

            using var context = CreateContext();
            var actual = context.Database
                .ExecuteSqlInterpolated(
                    $@"SELECT COUNT(*) FROM `Customers` WHERE `City` = {city} AND `ContactTitle` = {contactTitle}");

            Assert.Equal(DefaultSqlResult, actual);
        }

        protected override DbParameter CreateDbParameter(string name, object value)
            => new MySqlParameter
            {
                ParameterName = name,
                Value = value
            };

        protected override string TenMostExpensiveProductsSproc => @"CALL `Ten Most Expensive Products`()";

        protected override string CustomerOrderHistorySproc => @"CALL `CustOrderHist`(@CustomerID)";

        protected override string CustomerOrderHistoryWithGeneratedParameterSproc => @"CALL `CustOrderHist`({0})";
    }
}
