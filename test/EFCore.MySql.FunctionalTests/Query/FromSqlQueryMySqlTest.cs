using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySqlConnector;
using Microting.EntityFrameworkCore.MySql.Infrastructure;
using Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class FromSqlQueryMySqlTest : FromSqlQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public FromSqlQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CommonTableExpressions))]
        public override Task FromSqlRaw_composed_with_common_table_expression(bool async)
        {
            return base.FromSqlRaw_composed_with_common_table_expression(async);
        }

        public override async Task Multiple_occurrences_of_FromSql_with_db_parameter_adds_two_parameters(bool async)
        {
            // MySQL/MariaDB doesn't differentiate parameters based on Size attribute alone
            // Both parameters with the same name and value are treated as identical
            // So INTERSECT returns results instead of empty set
            using var context = CreateContext();
            var city = "Seattle";

            var dbParameter1 = CreateDbParameter("city", city);
            dbParameter1.Size = 7;
            var subquery1 = context.Set<Customer>()
                .FromSqlRaw(NormalizeDelimitersInRawString("SELECT * FROM [Customers] WHERE [City] = {0}"), dbParameter1);

            var dbParameter2 = CreateDbParameter("city", city);
            dbParameter2.Size = 3;
            var subquery2 = context.Set<Customer>()
                .FromSqlRaw(NormalizeDelimitersInRawString("SELECT * FROM [Customers] WHERE [City] = {0}"), dbParameter2);

            var query = subquery1.Intersect(subquery2);

            var actual = async
                ? await query.ToArrayAsync()
                : query.ToArray();

            // MySQL treats both parameters as identical, so INTERSECT returns results
            Assert.Single(actual);
            Assert.Equal("WHITC", actual[0].CustomerID);
        }

        protected override DbParameter CreateDbParameter(string name, object value)
            => new MySqlParameter
            {
                ParameterName = name,
                Value = value
            };
    }
}
