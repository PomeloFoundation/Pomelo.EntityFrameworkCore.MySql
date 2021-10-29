using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySqlConnector;
using Xunit;
using Xunit.Abstractions;

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

        protected override string TenMostExpensiveProductsSproc => "CALL `Ten Most Expensive Products`()";
        protected override string CustomerOrderHistorySproc => "CALL `CustOrderHist`({0})";

        private string NormalizeDelimitersInRawString(string sql)
            => Fixture.TestStore.NormalizeDelimitersInRawString(sql);

        public override async Task From_sql_queryable_stored_procedure_projection(bool async)
        {
            using var context = CreateContext();
            var query = context
                .Set<MostExpensiveProduct>()
                .FromSqlRaw(TenMostExpensiveProductsSproc, GetTenMostExpensiveProductsParameters())
                .Select(mep => mep.TenMostExpensiveProducts);

            if (async)
            {
                await Assert.ThrowsAsync<MySqlException>(() => query.ToArrayAsync());
            }
            else
            {
                Assert.Throws<MySqlException>(() => query.ToArray());
            }
        }

        public override async Task From_sql_queryable_stored_procedure_re_projection(bool async)
        {
            using var context = CreateContext();
            var query = context
                .Set<MostExpensiveProduct>()
                .FromSqlRaw(TenMostExpensiveProductsSproc, GetTenMostExpensiveProductsParameters())
                .Select(
                    mep =>
                        new MostExpensiveProduct { TenMostExpensiveProducts = "Foo", UnitPrice = mep.UnitPrice });

            if (async)
            {
                await Assert.ThrowsAsync<MySqlException>(() => query.ToArrayAsync());
            }
            else
            {
                Assert.Throws<MySqlException>(() => query.ToArray());
            }
        }

        public override async Task From_sql_queryable_stored_procedure_composed(bool async)
        {
            using var context = CreateContext();
            var query = context
                .Set<MostExpensiveProduct>()
                .FromSqlRaw(TenMostExpensiveProductsSproc, GetTenMostExpensiveProductsParameters())
                .Where(mep => mep.TenMostExpensiveProducts.Contains("C"))
                .OrderBy(mep => mep.UnitPrice);

            if (async)
            {
                await Assert.ThrowsAsync<MySqlException>(() => query.ToArrayAsync());
            }
            else
            {
                Assert.Throws<MySqlException>(() => query.ToArray());
            }
        }

        public override async Task From_sql_queryable_stored_procedure_with_parameter_composed(bool async)
        {
            using var context = CreateContext();

            var query = context
                .Set<CustomerOrderHistory>()
                .FromSqlRaw(CustomerOrderHistorySproc, GetCustomerOrderHistorySprocParameters())
                .Where(coh => coh.ProductName.Contains("C"))
                .OrderBy(coh => coh.Total);

            if (async)
            {
                await Assert.ThrowsAsync<MySqlException>(() => query.ToArrayAsync());
            }
            else
            {
                Assert.Throws<MySqlException>(() => query.ToArray());
            }
        }

        public override async Task From_sql_queryable_stored_procedure_take(bool async)
        {
            using var context = CreateContext();
            var query = context
                .Set<MostExpensiveProduct>()
                .FromSqlRaw(TenMostExpensiveProductsSproc, GetTenMostExpensiveProductsParameters())
                .OrderByDescending(mep => mep.UnitPrice)
                .Take(2);

            if (async)
            {
                await Assert.ThrowsAsync<MySqlException>(() => query.ToArrayAsync());
            }
            else
            {
                Assert.Throws<MySqlException>(() => query.ToArray());
            }
        }

        public override async Task From_sql_queryable_stored_procedure_min(bool async)
        {
            using var context = CreateContext();
            var query = context.Set<MostExpensiveProduct>()
                .FromSqlRaw(TenMostExpensiveProductsSproc, GetTenMostExpensiveProductsParameters());

            if (async)
            {
                await Assert.ThrowsAsync<MySqlException>(() => query.MinAsync(mep => mep.UnitPrice));
            }
            else
            {
                Assert.Throws<MySqlException>(() => query.Min(mep => mep.UnitPrice));
            }
        }

        public override async Task From_sql_queryable_stored_procedure_with_include_throws(bool async)
        {
            using var context = CreateContext();
            var query = context.Set<Product>()
                .FromSqlRaw("SelectStoredProcedure")
                .Include(p => p.OrderDetails);

            if (async)
            {
                await Assert.ThrowsAsync<MySqlException>(() => query.ToArrayAsync());
            }
            else
            {
                Assert.Throws<MySqlException>(() => query.ToArray());
            }
        }

        public override async Task From_sql_queryable_with_multiple_stored_procedures(bool async)
        {
            using var context = CreateContext();
            var query = from a in context.Set<MostExpensiveProduct>()
                    .FromSqlRaw(TenMostExpensiveProductsSproc, GetTenMostExpensiveProductsParameters())
                from b in context.Set<MostExpensiveProduct>()
                    .FromSqlRaw(TenMostExpensiveProductsSproc, GetTenMostExpensiveProductsParameters())
                where a.TenMostExpensiveProducts == b.TenMostExpensiveProducts
                select new { a, b };

            if (async)
            {
                await Assert.ThrowsAsync<MySqlException>(() => query.ToArrayAsync());
            }
            else
            {
                Assert.Throws<MySqlException>(() => query.ToArray());
            }
        }

        public override async Task From_sql_queryable_stored_procedure_and_select(bool async)
        {
            using var context = CreateContext();
            var query = from mep in context.Set<MostExpensiveProduct>()
                    .FromSqlRaw(TenMostExpensiveProductsSproc, GetTenMostExpensiveProductsParameters())
                from p in context.Set<Product>()
                    .FromSqlRaw(NormalizeDelimitersInRawString("SELECT * FROM [Products]"))
                where mep.TenMostExpensiveProducts == p.ProductName
                select new { mep, p };

            if (async)
            {
                await Assert.ThrowsAsync<MySqlException>(() => query.ToArrayAsync());
            }
            else
            {
                Assert.Throws<MySqlException>(() => query.ToArray());
            }
        }

        public override async Task From_sql_queryable_select_and_stored_procedure(bool async)
        {
            using var context = CreateContext();
            var query = from p in context.Set<Product>().FromSqlRaw(NormalizeDelimitersInRawString("SELECT * FROM [Products]"))
                from mep in context.Set<MostExpensiveProduct>()
                    .FromSqlRaw(TenMostExpensiveProductsSproc, GetTenMostExpensiveProductsParameters())
                where mep.TenMostExpensiveProducts == p.ProductName
                select new { mep, p };

            if (async)
            {
                await Assert.ThrowsAsync<MySqlException>(() => query.ToArrayAsync());
            }
            else
            {
                Assert.Throws<MySqlException>(() => query.ToArray());
            }
        }
    }
}
