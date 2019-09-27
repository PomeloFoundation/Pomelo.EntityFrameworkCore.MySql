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

        public override async Task<Exception> From_sql_queryable_stored_procedure_with_include_throws(bool async)
            => AssertSqlException(await base.From_sql_queryable_stored_procedure_with_include_throws(async));

        private static Exception AssertSqlException(Exception exception)
        {
            Assert.IsType<Exception>(exception);
            //Assert.Equal(102, ((SqlException)exception).Number);

            return exception;
        }

        public override async Task From_sql_queryable_stored_procedure(bool async)
        {
            await base.From_sql_queryable_stored_procedure(async);

            AssertSql("CALL `Ten Most Expensive Products`()");
        }

        public override async Task From_sql_queryable_stored_procedure_projection(bool async)
        {
            await base.From_sql_queryable_stored_procedure_projection(async);

            AssertSql("CALL `Ten Most Expensive Products`()");
        }

        public override async Task From_sql_queryable_stored_procedure_with_parameter(bool async)
        {
            await base.From_sql_queryable_stored_procedure_with_parameter(async);

            AssertSql(
                @"@p0='ALFKI' (Size = 4000)

CALL `CustOrderHist` (@p0)");
        }

        public override async Task From_sql_queryable_stored_procedure_re_projection_on_client(bool async)
        {
            await base.From_sql_queryable_stored_procedure_re_projection_on_client(async);

            AssertSql("CALL `Ten Most Expensive Products`()");
        }

        public override async Task<Exception> From_sql_queryable_stored_procedure_re_projection(bool async)
            => AssertSqlException(await base.From_sql_queryable_stored_procedure_re_projection(async));

        public override Task<Exception> From_sql_queryable_stored_procedure_composed(bool async)
        {
            var result = base.From_sql_queryable_stored_procedure_composed(async);

            AssertSql("CALL `Ten Most Expensive Products`()");

            return result;
        }

        public override async Task From_sql_queryable_stored_procedure_composed_on_client(bool async)
        {
            await base.From_sql_queryable_stored_procedure_composed_on_client(async);

            Assert.Equal(
                @"@p0='ALFKI' (Size = 4000)

CALL `CustOrderHist` (@p0)",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override async Task From_sql_queryable_stored_procedure_with_parameter_composed_on_client(bool async)
        {
            await base.From_sql_queryable_stored_procedure_with_parameter_composed_on_client(async);

            AssertSql(
                @"@p0='ALFKI' (Size = 4000)

CALL `CustOrderHist` (@p0)");
        }

        public override async Task<Exception> From_sql_queryable_stored_procedure_with_parameter_composed(bool async)
            => AssertSqlException(await base.From_sql_queryable_stored_procedure_with_parameter_composed(async));

        public override async Task From_sql_queryable_stored_procedure_take_on_client(bool async)
        {
            await base.From_sql_queryable_stored_procedure_take_on_client(async);

            AssertSql("CALL `Ten Most Expensive Products`()");
        }

        public override async Task<Exception> From_sql_queryable_stored_procedure_take(bool async)
            => AssertSqlException(await base.From_sql_queryable_stored_procedure_take(async));

        public override async Task From_sql_queryable_stored_procedure_min_on_client(bool async)
        {
            await base.From_sql_queryable_stored_procedure_min_on_client(async);

            AssertSql("CALL `Ten Most Expensive Products`()");
        }

        public override async Task From_sql_queryable_with_multiple_stored_procedures_on_client(bool async)
        {
            await base.From_sql_queryable_with_multiple_stored_procedures_on_client(async);

            Assert.StartsWith(
                @"CALL `Ten Most Expensive Products`()" + _eol +
                _eol +
                @"CALL `Ten Most Expensive Products`()" + _eol +
                _eol +
                @"CALL `Ten Most Expensive Products`()",
                Sql);
        }

        public override async Task<Exception> From_sql_queryable_with_multiple_stored_procedures(bool async)
            => AssertSqlException(await base.From_sql_queryable_with_multiple_stored_procedures(async));

        public override async Task From_sql_queryable_stored_procedure_and_select_on_client(bool async)
        {
            //await base.From_sql_queryable_stored_procedure_and_select_on_client(async);

            using (var context = CreateContext())
            {
                var actual
                    = await (from mep in context.Set<MostExpensiveProduct>()
                           .FromSqlRaw(TenMostExpensiveProductsSproc, GetTenMostExpensiveProductsParameters())
                       from p in context.Set<Product>().FromSqlRaw("SELECT * FROM `Products`")
                       where mep.TenMostExpensiveProducts == p.ProductName
                       select new { mep, p })
                    .ToArrayAsync();

                Assert.Equal(10, actual.Length);
            }

            Assert.StartsWith(
                @"CALL `Ten Most Expensive Products`()" + _eol +
                _eol +
                @"SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`" + _eol +
                @"FROM (" + _eol +
                @"    SELECT * FROM `Products`" + _eol +
                @") AS `p`" + _eol +
                _eol +
                @"SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`" + _eol +
                @"FROM (" + _eol +
                @"    SELECT * FROM `Products`" + _eol +
                @") AS `p`",
                Sql);
        }
        public override async Task<Exception> From_sql_queryable_stored_procedure_and_select(bool async)
            => AssertSqlException(await base.From_sql_queryable_stored_procedure_and_select(async));

        public override async Task From_sql_queryable_select_and_stored_procedure_on_client(bool async)
        {
            //await base.From_sql_queryable_select_and_stored_procedure_on_client(async);

            using (var context = CreateContext())
            {
                var actual
                    = await (from p in context.Set<Product>().FromSqlRaw("SELECT * FROM `Products`")
                       from mep in context.Set<MostExpensiveProduct>()
                           .FromSqlRaw(TenMostExpensiveProductsSproc, GetTenMostExpensiveProductsParameters())
                       where mep.TenMostExpensiveProducts == p.ProductName
                       select new { mep, p })
                    .ToArrayAsync();

                Assert.Equal(10, actual.Length);
            }

            Assert.StartsWith(
                @"SELECT `p`.`ProductID`, `p`.`Discontinued`, `p`.`ProductName`, `p`.`SupplierID`, `p`.`UnitPrice`, `p`.`UnitsInStock`" + _eol +
                @"FROM (" + _eol +
                @"    SELECT * FROM `Products`" + _eol +
                @") AS `p`" + _eol +
                _eol +
                @"CALL `Ten Most Expensive Products`()" + _eol +
                _eol +
                @"CALL `Ten Most Expensive Products`()",
                Sql);
        }

        public override async Task<Exception> From_sql_queryable_select_and_stored_procedure(bool async)
            => AssertSqlException(await base.From_sql_queryable_select_and_stored_procedure(async));

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override string TenMostExpensiveProductsSproc => "CALL `Ten Most Expensive Products`()";
        protected override string CustomerOrderHistorySproc => "CALL `CustOrderHist` ({0})";

        private static readonly string _eol = Environment.NewLine;

        private string Sql => Fixture.TestSqlLoggerFactory.Sql;
    }
}
