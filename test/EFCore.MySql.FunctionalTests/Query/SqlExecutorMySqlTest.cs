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

        public override void Executes_stored_procedure()
        {
            using var context = CreateContext();
            Assert.Equal(DefaultStoredProcedureResult, context.Database.ExecuteSqlRaw(TenMostExpensiveProductsSproc));
        }

        public override void Executes_stored_procedure_with_parameter()
        {
            using var context = CreateContext();
            var parameter = CreateDbParameter("@CustomerID", "ALFKI");

            Assert.Equal(DefaultStoredProcedureResult, context.Database.ExecuteSqlRaw(CustomerOrderHistorySproc, parameter));
        }

        public override void Executes_stored_procedure_with_generated_parameter()
        {
            using var context = CreateContext();
            Assert.Equal(DefaultStoredProcedureResult, context.Database.ExecuteSqlRaw(CustomerOrderHistoryWithGeneratedParameterSproc, "ALFKI"));
        }

        public override void Query_with_parameters_interpolated_2()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using var context = CreateContext();
            var actual = context.Database
                .ExecuteSql(
                    $@"SELECT COUNT(*) FROM `Customers` WHERE `City` = {city} AND `ContactTitle` = {contactTitle}");

            Assert.Equal(DefaultSqlResult, actual);
        }

        public override void Query_with_DbParameters_interpolated_2()
        {
            var city = CreateDbParameter("city", "London");
            var contactTitle = CreateDbParameter("contactTitle", "Sales Representative");

            using var context = CreateContext();
            var actual = context.Database
                .ExecuteSql(
                    $@"SELECT COUNT(*) FROM `Customers` WHERE `City` = {city} AND `ContactTitle` = {contactTitle}");

            Assert.Equal(DefaultSqlResult, actual);
        }

        public override async Task Executes_stored_procedure_async()
        {
            using var context = CreateContext();
            Assert.Equal(DefaultStoredProcedureResult, await context.Database.ExecuteSqlRawAsync(TenMostExpensiveProductsSproc));
        }

        public override async Task Executes_stored_procedure_with_parameter_async()
        {
            using var context = CreateContext();
            var parameter = CreateDbParameter("@CustomerID", "ALFKI");

            Assert.Equal(DefaultStoredProcedureResult, await context.Database.ExecuteSqlRawAsync(CustomerOrderHistorySproc, parameter));
        }

        public override async Task Executes_stored_procedure_with_generated_parameter_async()
        {
            using var context = CreateContext();
            Assert.Equal(DefaultStoredProcedureResult, await context.Database.ExecuteSqlRawAsync(CustomerOrderHistoryWithGeneratedParameterSproc, "ALFKI"));
        }

        public override async Task Query_with_parameters_interpolated_async_2()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using var context = CreateContext();
            var actual = await context.Database
                .ExecuteSqlAsync(
                    $@"SELECT COUNT(*) FROM `Customers` WHERE `City` = {city} AND `ContactTitle` = {contactTitle}");

            Assert.Equal(DefaultSqlResult, actual);
        }

        public override void Query_with_parameters()
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

        public override void Query_with_dbParameter_with_name()
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

        public override void Query_with_positional_dbParameter_with_name()
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

        public override void Query_with_positional_dbParameter_without_name()
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

        public override void Query_with_dbParameters_mixed()
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

        public override void Query_with_parameters_interpolated()
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

        public override async Task Query_with_parameters_async()
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

        public override async Task Query_with_parameters_interpolated_async()
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

        public override void Query_with_DbParameters_interpolated()
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
