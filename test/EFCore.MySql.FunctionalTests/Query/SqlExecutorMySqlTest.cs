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

        [ConditionalFact]
        public override void Query_with_parameters()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using (var context = CreateContext())
            {
                var actual = context.Database
                    .ExecuteSqlRaw(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = {0} AND `ContactTitle` = {1}", city, contactTitle);

                Assert.Equal(-1, actual);
            }
        }

        [Fact]
        public override void Query_with_dbParameter_with_name()
        {
            var city = CreateDbParameter("@city", "London");

            using (var context = CreateContext())
            {
                var actual = context.Database
                    .ExecuteSqlRaw(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = @city", city);

                Assert.Equal(-1, actual);
            }
        }

        [Fact]
        public override void Query_with_positional_dbParameter_with_name()
        {
            var city = CreateDbParameter("@city", "London");

            using (var context = CreateContext())
            {
                var actual = context.Database
                    .ExecuteSqlRaw(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = {0}", city);

                Assert.Equal(-1, actual);
            }
        }

        [Fact]
        public override void Query_with_positional_dbParameter_without_name()
        {
            var city = CreateDbParameter(name: null, value: "London");

            using (var context = CreateContext())
            {
                var actual = context.Database
                    .ExecuteSqlRaw(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = {0}", city);

                Assert.Equal(-1, actual);
            }
        }

        [Fact]
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

                Assert.Equal(-1, actual);

                actual = context.Database
                    .ExecuteSqlRaw(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = @city AND `ContactTitle` = {1}", cityParameter, contactTitle);

                Assert.Equal(-1, actual);
            }
        }

        [Fact]
        public override void Query_with_parameters_interpolated()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using (var context = CreateContext())
            {
                var actual = context.Database
                    .ExecuteSqlInterpolated(
                        $@"SELECT COUNT(*) FROM `Customers` WHERE `City` = {city} AND `ContactTitle` = {contactTitle}");

                Assert.Equal(-1, actual);
            }
        }

        [ConditionalFact]
        public override async Task Query_with_parameters_async()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using (var context = CreateContext())
            {
                var actual = await context.Database
                    .ExecuteSqlRawAsync(
                        @"SELECT COUNT(*) FROM `Customers` WHERE `City` = {0} AND `ContactTitle` = {1}", city, contactTitle);

                Assert.Equal(-1, actual);
            }
        }

        [Fact]
        public override async Task Query_with_parameters_interpolated_async()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using (var context = CreateContext())
            {
                var actual = await context.Database
                    .ExecuteSqlInterpolatedAsync(
                        $@"SELECT COUNT(*) FROM `Customers` WHERE `City` = {city} AND `ContactTitle` = {contactTitle}");

                Assert.Equal(-1, actual);
            }
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
