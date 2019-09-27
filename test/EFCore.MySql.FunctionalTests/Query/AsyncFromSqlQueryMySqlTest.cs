using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class AsyncFromSqlQueryMySqlTest : AsyncFromSqlQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public AsyncFromSqlQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }

        // Overrides can be removed when EF #11940 is fixed

        [Fact]
        public override async Task FromSqlRaw_queryable_simple()
        {
            using (var context = CreateContext())
            {
                var actual = await context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers` WHERE `ContactName` LIKE '%z%'")
                    .ToArrayAsync();

                Assert.Equal(14, actual.Length);
                Assert.Equal(14, context.ChangeTracker.Entries().Count());
            }
        }

        [Fact]
        public override async Task FromSqlRaw_queryable_simple_columns_out_of_order()
        {
            using (var context = CreateContext())
            {
                var actual = await context.Set<Customer>()
                    .FromSqlRaw(@"SELECT `Region`, `PostalCode`, `Phone`, `Fax`, `CustomerID`, `Country`, `ContactTitle`, `ContactName`, `CompanyName`, `City`, `Address` FROM `Customers`")
                    .ToArrayAsync();

                Assert.Equal(91, actual.Length);
                Assert.Equal(91, context.ChangeTracker.Entries().Count());
            }
        }

        [Fact]
        public override async Task FromSqlRaw_queryable_simple_columns_out_of_order_and_extra_columns()
        {
            using (var context = CreateContext())
            {
                var actual = await context.Set<Customer>()
                    .FromSqlRaw(@"SELECT `Region`, `PostalCode`, `PostalCode` AS `Foo`, `Phone`, `Fax`, `CustomerID`, `Country`, `ContactTitle`, `ContactName`, `CompanyName`, `City`, `Address` FROM `Customers`")
                    .ToArrayAsync();

                Assert.Equal(91, actual.Length);
                Assert.Equal(91, context.ChangeTracker.Entries().Count());
            }
        }

        [Fact]
        public override async Task FromSqlRaw_queryable_composed()
        {
            using (var context = CreateContext())
            {
                var actual = await context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers`")
                    .Where(c => c.ContactName.Contains("z"))
                    .ToArrayAsync();

                Assert.Equal(14, actual.Length);
            }
        }

        [Fact]
        public override async Task FromSqlRaw_queryable_multiple_composed()
        {
            using (var context = CreateContext())
            {
                var actual
                    = await (from c in context.Set<Customer>().FromSqlRaw(@"SELECT * FROM `Customers`")
                             from o in context.Set<Order>().FromSqlRaw(@"SELECT * FROM `Orders`")
                             where c.CustomerID == o.CustomerID
                             select new { c, o })
                        .ToArrayAsync();

                Assert.Equal(830, actual.Length);
            }
        }

        [Fact]
        public override async Task FromSqlRaw_queryable_multiple_composed_with_closure_parameters()
        {
            var startDate = new DateTime(1997, 1, 1);
            var endDate = new DateTime(1998, 1, 1);

            using (var context = CreateContext())
            {
                var actual
                    = await (from c in context.Set<Customer>().FromSqlRaw(@"SELECT * FROM `Customers`")
                             from o in context.Set<Order>().FromSqlRaw(
                                 @"SELECT * FROM `Orders` WHERE `OrderDate` BETWEEN {0} AND {1}",
                                 startDate,
                                 endDate)
                             where c.CustomerID == o.CustomerID
                             select new { c, o })
                        .ToArrayAsync();

                Assert.Equal(411, actual.Length);
            }
        }

        [Fact]
        public override async Task FromSqlRaw_queryable_multiple_composed_with_parameters_and_closure_parameters()
        {
            var city = "London";
            var startDate = new DateTime(1997, 1, 1);
            var endDate = new DateTime(1998, 1, 1);

            using (var context = CreateContext())
            {
                var actual
                    = await (from c in context.Set<Customer>().FromSqlRaw(@"SELECT * FROM `Customers` WHERE `City` = {0}", city)
                             from o in context.Set<Order>().FromSqlRaw(
                                 @"SELECT * FROM `Orders` WHERE `OrderDate` BETWEEN {0} AND {1}",
                                 startDate,
                                 endDate)
                             where c.CustomerID == o.CustomerID
                             select new { c, o })
                        .ToArrayAsync();

                Assert.Equal(25, actual.Length);
            }
        }

        [Fact]
        public override async Task FromSqlRaw_queryable_multiple_line_query()
        {
            using (var context = CreateContext())
            {
                var actual = await context.Set<Customer>()
                    .FromSqlRaw(
                        @"SELECT *
FROM `Customers`
WHERE `City` = 'London'")
                    .ToArrayAsync();

                Assert.Equal(6, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));
            }
        }

        [Fact]
        public override async Task FromSqlRaw_queryable_composed_multiple_line_query()
        {
            using (var context = CreateContext())
            {
                var actual = await context.Set<Customer>()
                    .FromSqlRaw(
                        @"SELECT *
FROM `Customers`")
                    .Where(c => c.City == "London")
                    .ToArrayAsync();

                Assert.Equal(6, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));
            }
        }

        [Fact]
        public override async Task FromSqlRaw_queryable_with_parameters()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using (var context = CreateContext())
            {
                var actual = await context.Set<Customer>()
                    .FromSqlRaw(
                        @"SELECT * FROM `Customers` WHERE `City` = {0} AND `ContactTitle` = {1}",
                        city,
                        contactTitle)
                    .ToArrayAsync();

                Assert.Equal(3, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));
                Assert.True(actual.All(c => c.ContactTitle == "Sales Representative"));
            }
        }

        [Fact]
        public override async Task FromSqlRaw_queryable_with_parameters_and_closure()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using (var context = CreateContext())
            {
                var actual = await context.Set<Customer>()
                    .FromSqlRaw(
                        @"SELECT * FROM `Customers` WHERE `City` = {0}",
                        city)
                    .Where(c => c.ContactTitle == contactTitle)
                    .ToArrayAsync();

                Assert.Equal(3, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));
                Assert.True(actual.All(c => c.ContactTitle == "Sales Representative"));
            }
        }

        [Fact]
        public override async Task FromSqlRaw_queryable_simple_cache_key_includes_query_string()
        {
            using (var context = CreateContext())
            {
                var actual = await context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers` WHERE `City` = 'London'")
                    .ToArrayAsync();

                Assert.Equal(6, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));

                actual = await context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers` WHERE `City` = 'Seattle'")
                    .ToArrayAsync();

                Assert.Equal(1, actual.Length);
                Assert.True(actual.All(c => c.City == "Seattle"));
            }
        }

        [Fact]
        public override async Task FromSqlRaw_queryable_with_parameters_cache_key_includes_parameters()
        {
            var city = "London";
            var contactTitle = "Sales Representative";
            var sql = @"SELECT * FROM `Customers` WHERE `City` = {0} AND `ContactTitle` = {1}";

            using (var context = CreateContext())
            {
                var actual = await context.Set<Customer>()
                    .FromSqlRaw(sql, city, contactTitle)
                    .ToArrayAsync();

                Assert.Equal(3, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));
                Assert.True(actual.All(c => c.ContactTitle == "Sales Representative"));

                city = "Madrid";
                contactTitle = "Accounting Manager";

                actual = await context.Set<Customer>()
                    .FromSqlRaw(sql, city, contactTitle)
                    .ToArrayAsync();

                Assert.Equal(2, actual.Length);
                Assert.True(actual.All(c => c.City == "Madrid"));
                Assert.True(actual.All(c => c.ContactTitle == "Accounting Manager"));
            }
        }

        [Fact]
        public override async Task FromSqlRaw_queryable_simple_as_no_tracking_not_composed()
        {
            using (var context = CreateContext())
            {
                var actual = await context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers`")
                    .AsNoTracking()
                    .ToArrayAsync();

                Assert.Equal(91, actual.Length);
                Assert.Equal(0, context.ChangeTracker.Entries().Count());
            }
        }

        [Fact]
        public override async Task FromSqlRaw_queryable_simple_projection_not_composed()
        {
            using (var context = CreateContext())
            {
                var actual = await context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers`")
                    .Select(c => new { c.CustomerID, c.City })
                    .AsNoTracking()
                    .ToArrayAsync();

                Assert.Equal(91, actual.Length);
                Assert.Equal(0, context.ChangeTracker.Entries().Count());
            }
        }

        [Fact]
        public override async Task FromSqlRaw_queryable_simple_include()
        {
            using (var context = CreateContext())
            {
                var actual = await context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers`")
                    .Include(c => c.Orders)
                    .ToArrayAsync();

                Assert.Equal(830, actual.SelectMany(c => c.Orders).Count());
            }
        }

        [Fact]
        public override async Task FromSqlRaw_queryable_simple_composed_include()
        {
            using (var context = CreateContext())
            {
                var actual = await context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers`")
                    .Where(c => c.City == "London")
                    .Include(c => c.Orders)
                    .ToArrayAsync();

                Assert.Equal(46, actual.SelectMany(c => c.Orders).Count());
            }
        }

        [Fact]
        public override async Task FromSqlRaw_annotations_do_not_affect_successive_calls()
        {
            using (var context = CreateContext())
            {
                var actual = await context.Customers
                    .FromSqlRaw(@"SELECT * FROM `Customers` WHERE `ContactName` LIKE '%z%'")
                    .ToArrayAsync();

                Assert.Equal(14, actual.Length);

                actual = await context.Customers
                    .ToArrayAsync();

                Assert.Equal(91, actual.Length);
            }
        }

        [Fact]
        public override async Task FromSqlRaw_composed_with_nullable_predicate()
        {
            using (var context = CreateContext())
            {
                var actual = await context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers`")
                    .Where(c => c.ContactName == c.CompanyName)
                    .ToArrayAsync();

                Assert.Equal(0, actual.Length);
            }
        }
    }
}
