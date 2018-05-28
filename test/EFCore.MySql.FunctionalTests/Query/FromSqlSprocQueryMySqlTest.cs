using System;
using System.Linq;
using EFCore.MySql.UpstreamFunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class FromSqlSprocQueryMySqlTest : FromSqlSprocQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public FromSqlSprocQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            fixture.TestSqlLoggerFactory.Clear();
        }

        public override void From_sql_queryable_stored_procedure()
        {
            base.From_sql_queryable_stored_procedure();

            Assert.Equal(
                "CALL `Ten Most Expensive Products`()",
                Sql);
        }

        public override void From_sql_queryable_stored_procedure_projection()
        {
            base.From_sql_queryable_stored_procedure_projection();

            Assert.Equal(
                "CALL `Ten Most Expensive Products`()",
                Sql);
        }

        public override void From_sql_queryable_stored_procedure_with_parameter()
        {
            base.From_sql_queryable_stored_procedure_with_parameter();

            Assert.Equal(
                @"@p0='ALFKI' (Size = 4000)

CALL `CustOrderHist` (@p0)",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void From_sql_queryable_stored_procedure_reprojection()
        {
            base.From_sql_queryable_stored_procedure_reprojection();

            Assert.Equal(
                "CALL `Ten Most Expensive Products`()",
                Sql);
        }

        public override void From_sql_queryable_stored_procedure_composed()
        {
            base.From_sql_queryable_stored_procedure_composed();

            Assert.Equal(
                "CALL `Ten Most Expensive Products`()",
                Sql);
        }

        public override void From_sql_queryable_stored_procedure_with_parameter_composed()
        {
            base.From_sql_queryable_stored_procedure_with_parameter_composed();

            Assert.Equal(
                @"@p0='ALFKI' (Size = 4000)

CALL `CustOrderHist` (@p0)",
                Sql,
                ignoreLineEndingDifferences: true);
        }

        public override void From_sql_queryable_stored_procedure_take()
        {
            base.From_sql_queryable_stored_procedure_take();

            Assert.Equal(
                "CALL `Ten Most Expensive Products`()",
                Sql);
        }

        public override void From_sql_queryable_stored_procedure_min()
        {
            base.From_sql_queryable_stored_procedure_min();

            Assert.Equal(
                "CALL `Ten Most Expensive Products`()",
                Sql);
        }

        public override void From_sql_queryable_with_multiple_stored_procedures()
        {
            base.From_sql_queryable_with_multiple_stored_procedures();

            Assert.StartsWith(
                @"CALL `Ten Most Expensive Products`()" + _eol +
                _eol +
                @"CALL `Ten Most Expensive Products`()" + _eol +
                _eol +
                @"CALL `Ten Most Expensive Products`()",
                Sql);
        }

        public override void From_sql_queryable_stored_procedure_and_select()
        {
            // base.From_sql_queryable_stored_procedure_and_select();

            using (var context = CreateContext())
            {
                var actual
                    = (from mep in context.Set<MostExpensiveProduct>()
                           .FromSql(TenMostExpensiveProductsSproc, GetTenMostExpensiveProductsParameters())
                       from p in context.Set<Product>().FromSql("SELECT * FROM `Products`")
                       where mep.TenMostExpensiveProducts == p.ProductName
                       select new { mep, p })
                    .ToArray();

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

        public override void From_sql_queryable_select_and_stored_procedure()
        {
            // base.From_sql_queryable_select_and_stored_procedure();
            using (var context = CreateContext())
            {
                var actual
                    = (from p in context.Set<Product>().FromSql("SELECT * FROM `Products`")
                       from mep in context.Set<MostExpensiveProduct>()
                           .FromSql(TenMostExpensiveProductsSproc, GetTenMostExpensiveProductsParameters())
                       where mep.TenMostExpensiveProducts == p.ProductName
                       select new { mep, p })
                    .ToArray();

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

        protected override string TenMostExpensiveProductsSproc => "CALL `Ten Most Expensive Products`()";
        protected override string CustomerOrderHistorySproc => "CALL `CustOrderHist` ({0})";

        private static readonly string _eol = Environment.NewLine;

        private string Sql => Fixture.TestSqlLoggerFactory.Sql;
    }
}
