using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySql.Data.MySqlClient;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class FromSqlQueryMySqlTest : FromSqlQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public FromSqlQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }

        protected override DbParameter CreateDbParameter(string name, object value)
            => new MySqlParameter
            {
                ParameterName = name,
                Value = value
            };

        private static readonly string _eol = Environment.NewLine;

        // Overrides can be removed when EF #11940 is fixed

        [Fact]
        public override void Bad_data_error_handling_invalid_cast_key()
        {
            using (var context = CreateContext())
            {
                Assert.Equal(
                    CoreStrings.ErrorMaterializingPropertyInvalidCast("Product", "ProductID", typeof(int), typeof(string)),
                    Assert.Throws<InvalidOperationException>(
                        () =>
                            context.Set<Product>()
                                .FromSqlRaw(
                                    @"SELECT `ProductID` AS `ProductName`, `ProductName` AS `ProductID`, `SupplierID`, `UnitPrice`, `UnitsInStock`, `Discontinued`
                               FROM `Products`")
                                .ToList()).Message);
            }
        }

        [Fact]
        public override void Bad_data_error_handling_invalid_cast()
        {
            using (var context = CreateContext())
            {
                Assert.Equal(
                    CoreStrings.ErrorMaterializingPropertyInvalidCast("Product", "UnitPrice", typeof(decimal?), typeof(int)),
                    Assert.Throws<InvalidOperationException>(
                        () =>
                            context.Set<Product>()
                                .FromSqlRaw(
                                    @"SELECT `ProductID`, `SupplierID` AS `UnitPrice`, `ProductName`, `SupplierID`, `UnitsInStock`, `Discontinued`
                               FROM `Products`")
                                .ToList()).Message);
            }
        }

        [Fact]
        public override void Bad_data_error_handling_invalid_cast_projection()
        {
            using (var context = CreateContext())
            {
                Assert.Equal(
                    CoreStrings.ErrorMaterializingPropertyInvalidCast("Product", "UnitPrice", typeof(decimal?), typeof(int)),
                    Assert.Throws<InvalidOperationException>(
                        () =>
                            context.Set<Product>()
                                .FromSqlRaw(
                                    @"SELECT `ProductID`, `SupplierID` AS `UnitPrice`, `ProductName`, `UnitsInStock`, `Discontinued`
                               FROM `Products`")
                                .Select(p => p.UnitPrice)
                                .ToList()).Message);
            }
        }

        [Fact]
        public override void Bad_data_error_handling_invalid_cast_no_tracking()
        {
            using (var context = CreateContext())
            {
                Assert.Equal(
                    CoreStrings.ErrorMaterializingPropertyInvalidCast("Product", "ProductID", typeof(int), typeof(string)),
                    Assert.Throws<InvalidOperationException>(
                        () =>
                        {
                            var set = (DbSet<Product>)context.Set<Product>()
                                .AsNoTracking();
                            return set
                                .FromSqlRaw(
                                    @"SELECT `ProductID` AS `ProductName`, `ProductName` AS `ProductID`, `SupplierID`, `UnitPrice`, `UnitsInStock`, `Discontinued`
                               FROM `Products`")
                                .ToList();
                        }
                        ).Message);
            }
        }

        [Fact]
        public override void Bad_data_error_handling_null()
        {
            using (var context = CreateContext())
            {
                Assert.Equal(
                    CoreStrings.ErrorMaterializingPropertyNullReference("Product", "Discontinued", typeof(bool)),
                    Assert.Throws<InvalidOperationException>(
                        () =>
                            context.Set<Product>()
                                .FromSqlRaw(
                                    @"SELECT `ProductID`, `ProductName`, `SupplierID`, `UnitPrice`, `UnitsInStock`, NULL AS `Discontinued`
                               FROM `Products`")
                                .ToList()).Message);
            }
        }

        [Fact]
        public override void Bad_data_error_handling_null_projection()
        {
            using (var context = CreateContext())
            {
                Assert.Equal(
                    CoreStrings.ErrorMaterializingPropertyNullReference("Product", "Discontinued", typeof(bool)),
                    Assert.Throws<InvalidOperationException>(
                        () =>
                            context.Set<Product>()
                                .FromSqlRaw(
                                    @"SELECT `ProductID`, `ProductName`, `SupplierID`, `UnitPrice`, `UnitsInStock`, NULL AS `Discontinued`
                               FROM `Products`")
                                .Select(p => p.Discontinued)
                                .ToList()).Message);
            }
        }

        [Fact]
        public override void Bad_data_error_handling_null_no_tracking()
        {
            using (var context = CreateContext())
            {
                Assert.Equal(
                    CoreStrings.ErrorMaterializingPropertyNullReference("Product", "Discontinued", typeof(bool)),
                    Assert.Throws<InvalidOperationException>(
                        () =>
                        {
                            var set = (DbSet<Product>)context.Set<Product>()
                                .AsNoTracking();
                            return set
                                .FromSqlRaw(
                                    @"SELECT `ProductID`, `ProductName`, `SupplierID`, `UnitPrice`, `UnitsInStock`, NULL AS `Discontinued`
                               FROM `Products`")
                                .ToList();
                        }
                        ).Message);
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_simple()
        {
            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers` WHERE `ContactName` LIKE '%z%'")
                    .ToArray();

                Assert.Equal(14, actual.Length);
                Assert.Equal(14, context.ChangeTracker.Entries().Count());
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_simple_columns_out_of_order()
        {
            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(@"SELECT `Region`, `PostalCode`, `Phone`, `Fax`, `CustomerID`, `Country`, `ContactTitle`, `ContactName`, `CompanyName`, `City`, `Address` FROM `Customers`")
                    .ToArray();

                Assert.Equal(91, actual.Length);
                Assert.Equal(91, context.ChangeTracker.Entries().Count());
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_simple_columns_out_of_order_and_extra_columns()
        {
            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(@"SELECT `Region`, `PostalCode`, `PostalCode` AS `Foo`, `Phone`, `Fax`, `CustomerID`, `Country`, `ContactTitle`, `ContactName`, `CompanyName`, `City`, `Address` FROM `Customers`")
                    .ToArray();

                Assert.Equal(91, actual.Length);
                Assert.Equal(91, context.ChangeTracker.Entries().Count());
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_simple_columns_out_of_order_and_not_enough_columns_throws()
        {
            using (var context = CreateContext())
            {
                Assert.Equal(
                    RelationalStrings.FromSqlMissingColumn("Region"),
                    Assert.Throws<InvalidOperationException>(
                        () => context.Set<Customer>()
                            .FromSqlRaw(@"SELECT `PostalCode`, `Phone`, `Fax`, `CustomerID`, `Country`, `ContactTitle`, `ContactName`, `CompanyName`, `City`, `Address` FROM `Customers`")
                            .ToArray()
                    ).Message);
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_composed()
        {
            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers`")
                    .Where(c => c.ContactName.Contains("z"))
                    .ToArray();

                Assert.Equal(14, actual.Length);
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_composed_after_removing_whitespaces()
        {
            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(
                        _eol +
                        @"    " + _eol +
                        _eol +
                        _eol +
                        @"SELECT" + _eol +
                        @"* FROM `Customers`")
                    .Where(c => c.ContactName.Contains("z"))
                    .ToArray();

                Assert.Equal(14, actual.Length);
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_composed_compiled()
        {
            var query = EF.CompileQuery(
                (NorthwindContext context) => context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers`")
                    .Where(c => c.ContactName.Contains("z")));

            using (var context = CreateContext())
            {
                var actual = query(context).ToArray();

                Assert.Equal(14, actual.Length);
            }
        }

        [Fact]
        public override void FromSqlRaw_composed_contains()
        {
            using (var context = CreateContext())
            {
                var actual
                    = (from c in context.Set<Customer>()
                       where context.Orders.FromSqlRaw(@"SELECT * FROM `Orders`")
                           .Select(o => o.CustomerID)
                           .Contains(c.CustomerID)
                       select c)
                    .ToArray();

                Assert.Equal(89, actual.Length);
            }
        }

        [Fact]
        public override void FromSqlRaw_composed_contains2()
        {
            using (var context = CreateContext())
            {
                var actual
                    = (from c in context.Set<Customer>()
                       where
                           c.CustomerID == "ALFKI"
                           && context.Orders.FromSqlRaw(@"SELECT * FROM `Orders`")
                               .Select(o => o.CustomerID)
                               .Contains(c.CustomerID)
                       select c)
                    .ToArray();

                Assert.Equal(1, actual.Length);
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_multiple_composed()
        {
            using (var context = CreateContext())
            {
                var actual
                    = (from c in context.Set<Customer>().FromSqlRaw(@"SELECT * FROM `Customers`")
                       from o in context.Set<Order>().FromSqlRaw(@"SELECT * FROM `Orders`")
                       where c.CustomerID == o.CustomerID
                       select new { c, o })
                    .ToArray();

                Assert.Equal(830, actual.Length);
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_multiple_composed_with_closure_parameters()
        {
            var startDate = new DateTime(1997, 1, 1);
            var endDate = new DateTime(1998, 1, 1);

            using (var context = CreateContext())
            {
                var actual
                    = (from c in context.Set<Customer>().FromSqlRaw(@"SELECT * FROM `Customers`")
                       from o in context.Set<Order>().FromSqlRaw(
                           @"SELECT * FROM `Orders` WHERE `OrderDate` BETWEEN {0} AND {1}",
                           startDate,
                           endDate)
                       where c.CustomerID == o.CustomerID
                       select new { c, o })
                    .ToArray();

                Assert.Equal(411, actual.Length);
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_multiple_composed_with_parameters_and_closure_parameters()
        {
            var city = "London";
            var startDate = new DateTime(1997, 1, 1);
            var endDate = new DateTime(1998, 1, 1);

            using (var context = CreateContext())
            {
                var actual
                    = (from c in context.Set<Customer>().FromSqlRaw(@"SELECT * FROM `Customers` WHERE `City` = {0}", city)
                       from o in context.Set<Order>().FromSqlRaw(
                           @"SELECT * FROM `Orders` WHERE `OrderDate` BETWEEN {0} AND {1}",
                           startDate,
                           endDate)
                       where c.CustomerID == o.CustomerID
                       select new { c, o })
                    .ToArray();

                Assert.Equal(25, actual.Length);

                city = "Berlin";
                startDate = new DateTime(1998, 4, 1);
                endDate = new DateTime(1998, 5, 1);

                actual
                    = (from c in context.Set<Customer>().FromSqlRaw(@"SELECT * FROM `Customers` WHERE `City` = {0}", city)
                       from o in context.Set<Order>().FromSqlRaw(
                           @"SELECT * FROM `Orders` WHERE `OrderDate` BETWEEN {0} AND {1}",
                           startDate,
                           endDate)
                       where c.CustomerID == o.CustomerID
                       select new { c, o })
                    .ToArray();

                Assert.Equal(1, actual.Length);
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_multiple_line_query()
        {
            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(
                        @"SELECT *
FROM `Customers`
WHERE `City` = 'London'")
                    .ToArray();

                Assert.Equal(6, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_composed_multiple_line_query()
        {
            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(
                        @"SELECT *
FROM `Customers`")
                    .Where(c => c.City == "London")
                    .ToArray();

                Assert.Equal(6, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_with_parameters()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(
                        @"SELECT * FROM `Customers` WHERE `City` = {0} AND `ContactTitle` = {1}",
                        city,
                        contactTitle)
                    .ToArray();

                Assert.Equal(3, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));
                Assert.True(actual.All(c => c.ContactTitle == "Sales Representative"));
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_with_parameters_inline()
        {
            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(
                        @"SELECT * FROM `Customers` WHERE `City` = {0} AND `ContactTitle` = {1}",
                        "London",
                        "Sales Representative")
                    .ToArray();

                Assert.Equal(3, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));
                Assert.True(actual.All(c => c.ContactTitle == "Sales Representative"));
            }
        }

        [Fact]
        public override void FromSqlInterpolated_queryable_with_parameters_interpolated()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(
                        $@"SELECT * FROM `Customers` WHERE `City` = {city} AND `ContactTitle` = {contactTitle}")
                    .ToArray();

                Assert.Equal(3, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));
                Assert.True(actual.All(c => c.ContactTitle == "Sales Representative"));
            }
        }

        [Fact]
        public override void FromSqlInterpolated_queryable_with_parameters_inline_interpolated()
        {
            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(
                        $@"SELECT * FROM `Customers` WHERE `City` = {"London"} AND `ContactTitle` = {"Sales Representative"}")
                    .ToArray();

                Assert.Equal(3, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));
                Assert.True(actual.All(c => c.ContactTitle == "Sales Representative"));
            }
        }

        [Fact]
        public override void FromSqlInterpolated_queryable_multiple_composed_with_parameters_and_closure_parameters_interpolated()
        {
            var city = "London";
            var startDate = new DateTime(1997, 1, 1);
            var endDate = new DateTime(1998, 1, 1);

            using (var context = CreateContext())
            {
                var actual
                    = (from c in context.Set<Customer>().FromSqlRaw(@"SELECT * FROM `Customers` WHERE `City` = {0}", city)
                       from o in context.Set<Order>().FromSqlRaw($@"SELECT * FROM `Orders` WHERE `OrderDate` BETWEEN {startDate} AND {endDate}")
                       where c.CustomerID == o.CustomerID
                       select new { c, o })
                    .ToArray();

                Assert.Equal(25, actual.Length);

                city = "Berlin";
                startDate = new DateTime(1998, 4, 1);
                endDate = new DateTime(1998, 5, 1);

                actual
                    = (from c in context.Set<Customer>().FromSqlRaw(@"SELECT * FROM `Customers` WHERE `City` = {0}", city)
                       from o in context.Set<Order>().FromSqlRaw($@"SELECT * FROM `Orders` WHERE `OrderDate` BETWEEN {startDate} AND {endDate}")
                       where c.CustomerID == o.CustomerID
                       select new { c, o })
                    .ToArray();

                Assert.Equal(1, actual.Length);
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_with_null_parameter()
        {
            uint? reportsTo = null;

            using (var context = CreateContext())
            {
                var actual = context.Set<Employee>()
                    .FromSqlRaw(
                        @"SELECT * FROM `Employees` WHERE `ReportsTo` = {0} OR (`ReportsTo` IS NULL AND {0} IS NULL)",
                        // ReSharper disable once ExpressionIsAlwaysNull
                        reportsTo)
                    .ToArray();

                Assert.Equal(1, actual.Length);
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_with_parameters_and_closure()
        {
            var city = "London";
            var contactTitle = "Sales Representative";

            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers` WHERE `City` = {0}", city)
                    .Where(c => c.ContactTitle == contactTitle)
                    .ToArray();

                Assert.Equal(3, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));
                Assert.True(actual.All(c => c.ContactTitle == "Sales Representative"));
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_simple_cache_key_includes_query_string()
        {
            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers` WHERE `City` = 'London'")
                    .ToArray();

                Assert.Equal(6, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));

                actual = context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers` WHERE `City` = 'Seattle'")
                    .ToArray();

                Assert.Equal(1, actual.Length);
                Assert.True(actual.All(c => c.City == "Seattle"));
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_with_parameters_cache_key_includes_parameters()
        {
            var city = "London";
            var contactTitle = "Sales Representative";
            var sql = @"SELECT * FROM `Customers` WHERE `City` = {0} AND `ContactTitle` = {1}";

            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(sql, city, contactTitle)
                    .ToArray();

                Assert.Equal(3, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));
                Assert.True(actual.All(c => c.ContactTitle == "Sales Representative"));

                city = "Madrid";
                contactTitle = "Accounting Manager";

                actual = context.Set<Customer>()
                    .FromSqlRaw(sql, city, contactTitle)
                    .ToArray();

                Assert.Equal(2, actual.Length);
                Assert.True(actual.All(c => c.City == "Madrid"));
                Assert.True(actual.All(c => c.ContactTitle == "Accounting Manager"));
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_simple_as_no_tracking_not_composed()
        {
            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers`")
                    .AsNoTracking()
                    .ToArray();

                Assert.Equal(91, actual.Length);
                Assert.Equal(0, context.ChangeTracker.Entries().Count());
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_simple_projection_composed()
        {
            // No Sprocs
        }

        [Fact]
        public override void FromSqlRaw_queryable_simple_include()
        {
            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers`")
                    .Include(c => c.Orders)
                    .ToArray();

                Assert.Equal(830, actual.SelectMany(c => c.Orders).Count());
            }
        }

        [Fact]
        public override void FromSqlRaw_queryable_simple_composed_include()
        {
            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers`")
                    .Where(c => c.City == "London")
                    .Include(c => c.Orders)
                    .ToArray();

                Assert.Equal(46, actual.SelectMany(c => c.Orders).Count());
            }
        }

        [Fact]
        public override void FromSqlRaw_annotations_do_not_affect_successive_calls()
        {
            using (var context = CreateContext())
            {
                var actual = context.Customers
                    .FromSqlRaw(@"SELECT * FROM `Customers` WHERE `ContactName` LIKE '%z%'")
                    .ToArray();

                Assert.Equal(14, actual.Length);

                actual = context.Customers
                    .ToArray();

                Assert.Equal(91, actual.Length);
            }
        }

        [Fact]
        public override void FromSqlRaw_composed_with_nullable_predicate()
        {
            using (var context = CreateContext())
            {
                var actual = context.Set<Customer>()
                    .FromSqlRaw(@"SELECT * FROM `Customers`")
                    .Where(c => c.ContactName == c.CompanyName)
                    .ToArray();

                Assert.Equal(0, actual.Length);
            }
        }

        [Fact]
        public override void FromSqlRaw_with_dbParameter()
        {
            using (var context = CreateContext())
            {
                var parameter = CreateDbParameter("@city", "London");

                var actual = context.Customers
                    .FromSqlRaw(@"SELECT * FROM `Customers` WHERE `City` = @city", parameter)
                    .ToArray();

                Assert.Equal(6, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));
            }
        }

        [Fact]
        public override void FromSqlRaw_with_dbParameter_mixed()
        {
            using (var context = CreateContext())
            {
                var city = "London";
                var title = "Sales Representative";

                var titleParameter = CreateDbParameter("@title", title);

                var actual = context.Customers
                    .FromSqlRaw(
                        @"SELECT * FROM `Customers` WHERE `City` = {0} AND `ContactTitle` = @title",
                        city,
                        titleParameter)
                    .ToArray();

                Assert.Equal(3, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));
                Assert.True(actual.All(c => c.ContactTitle == "Sales Representative"));

                var cityParameter = CreateDbParameter("@city", city);

                actual = context.Customers
                    .FromSqlRaw(
                        @"SELECT * FROM `Customers` WHERE `City` = @city AND `ContactTitle` = {1}",
                        cityParameter,
                        title)
                    .ToArray();

                Assert.Equal(3, actual.Length);
                Assert.True(actual.All(c => c.City == "London"));
                Assert.True(actual.All(c => c.ContactTitle == "Sales Representative"));
            }
        }

        [Fact]
        public override void Include_does_not_close_user_opened_connection_for_empty_result()
        {
            using (var context = CreateContext())
            {
                context.Database.OpenConnection();

                var query = context.Customers
                    .Include(v => v.Orders)
                    .Where(v => v.CustomerID == "MAMRFC")
                    .ToList();

                Assert.Empty(query);
                Assert.Equal(ConnectionState.Open, context.Database.GetDbConnection().State);
            }
        }

        [Fact]
        public override void FromSqlRaw_with_db_parameters_called_multiple_times()
        {
            using (var context = CreateContext())
            {
                var parameter = CreateDbParameter("@id", "ALFKI");

                var query = context.Customers
                    .FromSqlRaw(@"SELECT * FROM `Customers` WHERE `CustomerID` = @id", parameter);

                // ReSharper disable PossibleMultipleEnumeration
                var result1 = query.ToList();

                Assert.Equal(1, result1.Count);

                var result2 = query.ToList();
                // ReSharper restore PossibleMultipleEnumeration

                Assert.Equal(1, result2.Count);
            }
        }

        [Fact]
        public override void FromSqlRaw_with_SelectMany_and_include()
        {
            using (var context = CreateContext())
            {
                var query = from c1 in context.Set<Customer>()
                                .FromSqlRaw(@"SELECT * FROM `Customers` WHERE `CustomerID` = 'ALFKI'")
                            from c2 in context.Set<Customer>()
                                .FromSqlRaw(@"SELECT * FROM `Customers` WHERE `CustomerID` = 'AROUT'")
                                .Include(c => c.Orders)
                            select new { c1, c2 };

                var result = query.ToList();
                Assert.Equal(1, result.Count);

                var customers1 = result.Select(r => r.c1);
                var customers2 = result.Select(r => r.c2);
                foreach (var customer1 in customers1)
                {
                    Assert.Null(customer1.Orders);
                }

                foreach (var customer2 in customers2)
                {
                    Assert.NotNull(customer2.Orders);
                }
            }
        }

        [Fact]
        public override void FromSqlRaw_with_join_and_include()
        {
            using (var context = CreateContext())
            {
                var query = from c in context.Set<Customer>().FromSqlRaw(@"SELECT * FROM `Customers` WHERE `CustomerID` = 'ALFKI'")
                            join o in context.Set<Order>().FromSqlRaw(@"SELECT * FROM `Orders` WHERE `OrderID` <> 1").Include(o => o.OrderDetails)
                                on c.CustomerID equals o.CustomerID
                            select new { c, o };

                var result = query.ToList();

                Assert.Equal(6, result.Count);

                var orders = result.Select(r => r.o);
                foreach (var order in orders)
                {
                    Assert.NotNull(order.OrderDetails);
                }
            }
        }
    }
}
