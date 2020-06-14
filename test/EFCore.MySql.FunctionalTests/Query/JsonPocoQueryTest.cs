using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class JsonPocoQueryTest : IClassFixture<JsonPocoQueryTest.JsonPocoQueryFixture>
    {
        JsonPocoQueryFixture Fixture { get; }

        public JsonPocoQueryTest(JsonPocoQueryFixture fixture, ITestOutputHelper testOutputHelper)
        {
            Fixture = fixture;
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [Fact]
        public void Roundtrip()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.Id == 1);
            var customer = x.Customer;

            Assert.Equal("Joe", customer.Name);
            Assert.Equal(25, customer.Age);

            var orders = customer.Orders;

            Assert.Equal(99.5m, orders[0].Price);
            Assert.Equal("Some address 1", orders[0].ShippingAddress);
            Assert.Equal(new DateTime(2019, 10, 1), orders[0].ShippingDate);
            Assert.Equal(23, orders[1].Price);
            Assert.Equal("Some address 2", orders[1].ShippingAddress);
            Assert.Equal(new DateTime(2019, 10, 10), orders[1].ShippingDate);
        }

        [Fact]
        public void Roundtrip_json()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.Id == 1);
            var customer = x.Customer;

            Assert.Equal("Joe", customer.Name);
            Assert.Equal(25, customer.Age);

            var orders = customer.Orders;

            Assert.Equal(99.5m, orders[0].Price);
            Assert.Equal("Some address 1", orders[0].ShippingAddress);
            Assert.Equal(new DateTime(2019, 10, 1), orders[0].ShippingDate);
            Assert.Equal(23, orders[1].Price);
            Assert.Equal("Some address 2", orders[1].ShippingAddress);
            Assert.Equal(new DateTime(2019, 10, 10), orders[1].ShippingDate);
        }

        [Fact]
        public void Literal()
        {
            using var ctx = CreateContext();

            Assert.Empty(ctx.JsonEntities.Where(e => e.Customer == new Customer { Name = "Test customer", Age = 80 }));
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE `j`.`Customer` = '{""Name"":""Test customer"",""Age"":80,""ID"":""00000000-0000-0000-0000-000000000000"",""is_vip"":false,""Statistics"":null,""Orders"":null}'");
        }

        [Fact]
        public void Parameter()
        {
            using var ctx = CreateContext();
            var expected = ctx.JsonEntities.Find(1).Customer;
            var actual = ctx.JsonEntities.Single(e => e.Customer == expected).Customer;

            Assert.Equal(actual.Name, expected.Name);
            AssertSql(
                @"@__p_0='1'

SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE `j`.`Id` = @__p_0
LIMIT 1",
                //
                @"@__expected_0='{""Name"":""Joe"",""Age"":25,""ID"":""00000000-0000-0000-0000-000000000000"",""is_vip"":false,""Statistics"":{""Visits"":4,""Purchases"":3,""Nested"":{""SomeProperty"":10,""SomeNullableInt"":20,""IntArray"":[3,4],""SomeNullableGuid"":""d5f2685d-e5c4-47e5-97aa-d0266154eb2d""}},""Orders"":[{""Price"":99.5,""ShippingAddress"":""Some address 1"",""ShippingDate"":""2019-10-01T00:00:00""},{""Price"":23,""ShippingAddress"":""Some address 2"",""ShippingDate"":""2019-10-10T00:00:00""}]}'

SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE `j`.`Customer` = CAST(@__expected_0 AS json)
LIMIT 2");
        }

        [Fact]
        public void Text_output()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.Customer.Name == "Joe");

            Assert.Equal("Joe", x.Customer.Name);
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE `j`.`Customer`->>'$.Name' = 'Joe'
LIMIT 2");
        }

        [Fact]
        public void Text_output_json()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.Customer.Name == "Joe");

            Assert.Equal("Joe", x.Customer.Name);
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE `j`.`Customer`->>'$.Name' = 'Joe'
LIMIT 2");
        }

        [Fact]
        public void Integer_output()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.Customer.Age < 30);

            Assert.Equal("Joe", x.Customer.Name);
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE `j`.`Customer`->'$.Age' < 30
LIMIT 2");
        }

        [Fact]
        public void Guid_output()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.Customer.ID == Guid.Empty);

            Assert.Equal("Joe", x.Customer.Name);
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE `j`.`Customer`->'$.ID' = '00000000-0000-0000-0000-000000000000'
LIMIT 2");
        }

        [Fact]
        public void Bool_output()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.Customer.IsVip);

            Assert.Equal("Moe", x.Customer.Name);
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE `j`.`Customer`->'$.is_vip' = TRUE
LIMIT 2");
        }

        [Fact]
        public void Nullable()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.Customer.Statistics.Nested.SomeNullableInt == 20);

            Assert.Equal("Joe", x.Customer.Name);

            AssertSql(
                @"SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE `j`.`Customer`->'$.Statistics.Nested.SomeNullableInt' = 20
LIMIT 2");
        }

        [Fact]
        public void Nested()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.Customer.Statistics.Visits == 4);

            Assert.Equal("Joe", x.Customer.Name);
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE `j`.`Customer`->'$.Statistics.Visits' = 4
LIMIT 2");
        }

        [Fact]
        public void Nested_twice()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.Customer.Statistics.Nested.SomeProperty == 10);

            Assert.Equal("Joe", x.Customer.Name);
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE `j`.`Customer`->'$.Statistics.Nested.SomeProperty' = 10
LIMIT 2");
        }

        [Fact]
        public void Array_of_objects()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.Customer.Orders[0].Price == 99.5m);

            Assert.Equal("Joe", x.Customer.Name);
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE `j`.`Customer`->'$.Orders[0].Price' = 99.5
LIMIT 2");
        }

        [Fact]
        public void Array_toplevel()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.ToplevelArray[1] == "two");

            Assert.Equal("Joe", x.Customer.Name);
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE `j`.`ToplevelArray`->>'$[1]' = 'two'
LIMIT 2");
        }

        [Fact]
        public void Array_nested()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.Customer.Statistics.Nested.IntArray[1] == 4);

            Assert.Equal("Joe", x.Customer.Name);
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE `j`.`Customer`->'$.Statistics.Nested.IntArray[1]' = 4
LIMIT 2");
        }

        [Fact]
        public void Array_parameter_index()
        {
            using var ctx = CreateContext();
            var i = 1;
            var x = ctx.JsonEntities.Single(e => e.Customer.Statistics.Nested.IntArray[i] == 4);

            Assert.Equal("Joe", x.Customer.Name);
            AssertSql(
                @"@__i_0='1'

SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`Customer`, CONCAT('$.Statistics.Nested.IntArray[', @__i_0, ']')) = 4
LIMIT 2");
        }

        [Fact]
        public void Array_Length()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.Customer.Orders.Length == 2);

            Assert.Equal("Joe", x.Customer.Name);
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE JSON_LENGTH(`j`.`Customer`->'$.Orders') = 2
LIMIT 2");
        }

        [Fact]
        public void Array_Any_toplevel()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.ToplevelArray.Any());

            Assert.Equal("Joe", x.Customer.Name);
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE JSON_LENGTH(`j`.`ToplevelArray`) > 0
LIMIT 2");
        }

        [Fact]
        public void Like()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.Customer.Name.StartsWith("J"));

            Assert.Equal("Joe", x.Customer.Name);
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE `j`.`Customer`->>'$.Name' IS NOT NULL AND (`j`.`Customer`->>'$.Name' LIKE 'J%')
LIMIT 2");
//             AssertSql(
//                 @"SELECT j.""Id"", j.""Customer"", j.""ToplevelArray""
// FROM ""JsonEntities"" AS j
// WHERE j.""Customer""->>'Name' LIKE 'J%'
// LIMIT 2");
        }

        [Fact] // #1363
        public void Where_nullable_guid()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e =>
                e.Customer.Statistics.Nested.SomeNullableGuid == Guid.Parse("d5f2685d-e5c4-47e5-97aa-d0266154eb2d"));

            Assert.Equal("Joe", x.Customer.Name);
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`Customer`, `j`.`ToplevelArray`
FROM `JsonEntities` AS `j`
WHERE `j`.`Customer`->'$.Statistics.Nested.SomeNullableGuid' = 'd5f2685d-e5c4-47e5-97aa-d0266154eb2d'
LIMIT 2");
        }

        #region Functions

        [Fact]
        public void JsonContains_with_json_element()
        {
            using var ctx = CreateContext();
            var element = JsonDocument.Parse(@"{""Name"": ""Joe"", ""Age"": 25}").RootElement;
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContains(e.Customer, element));

            Assert.Equal(1, count);
            AssertSql(
                @"@__element_1='{""Name"":""Joe"",""Age"":25}' (Nullable = false)

SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS(`j`.`Customer`, CAST(@__element_1 AS json))");
        }

        [Fact]
        public void JsonContains_with_string_literal()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContains(e.Customer, @"{""Name"": ""Joe"", ""Age"": 25}"));

            Assert.Equal(1, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS(`j`.`Customer`, '{""Name"": ""Joe"", ""Age"": 25}')");
        }

        [Fact]
        public void JsonContains_with_string_parameter()
        {
            using var ctx = CreateContext();
            var someJson = @"{""Name"": ""Joe"", ""Age"": 25}";
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContains(e.Customer, someJson));

            Assert.Equal(1, count);
            AssertSql(
                @"@__someJson_1='{""Name"": ""Joe""
""Age"": 25}' (Size = 4000)

SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS(`j`.`Customer`, @__someJson_1)");
        }

        [Fact]
        public void JsonContainsPath()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContainsPath(e.Customer, "$.Statistics.Visits"));

            Assert.Equal(2, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS_PATH(`j`.`Customer`, 'one', '$.Statistics.Visits')");
        }

        [Fact]
        public void JsonContainsPathAny()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContainsPathAny(e.Customer, "$.Statistics.foo", "$.Statistics.Visits"));

            Assert.Equal(2, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS_PATH(`j`.`Customer`, 'one', '$.Statistics.foo', '$.Statistics.Visits')");
        }

        [Fact]
        public void JsonExistAll()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContainsPathAll(e.Customer, "$.Statistics.foo", "$.Statistics.Visits"));

            Assert.Equal(0, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS_PATH(`j`.`Customer`, 'all', '$.Statistics.foo', '$.Statistics.Visits')");
        }

        [Fact]
        public void JsonType()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonType(e.Customer.Statistics.Visits) == "INTEGER");

            Assert.Equal(2, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_TYPE(`j`.`Customer`->'$.Statistics.Visits') = 'INTEGER'");
        }

        #endregion Functions

        #region Support

        protected JsonPocoQueryContext CreateContext() => Fixture.CreateContext();

        void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class JsonPocoQueryContext : PoolableDbContext
        {
            public DbSet<JsonEntity> JsonEntities { get; set; }

            public JsonPocoQueryContext(DbContextOptions options) : base(options) {}

            public static void Seed(JsonPocoQueryContext context)
            {
                context.JsonEntities.AddRange(
                    new JsonEntity { Id = 1, Customer = CreateCustomer1(), ToplevelArray = new[] { "one", "two", "three" } },
                    new JsonEntity { Id = 2, Customer = CreateCustomer2() });
                context.SaveChanges();

                static Customer CreateCustomer1() => new Customer
                {
                    Name = "Joe",
                    Age = 25,
                    ID = Guid.Empty,
                    IsVip = false,
                    Statistics = new Statistics
                    {
                        Visits = 4,
                        Purchases = 3,
                        Nested = new NestedStatistics
                        {
                            SomeProperty = 10,
                            SomeNullableInt = 20,
                            SomeNullableGuid = Guid.Parse("d5f2685d-e5c4-47e5-97aa-d0266154eb2d"),
                            IntArray = new[] { 3, 4 }
                        }
                    },
                    Orders = new[]
                    {
                        new Order
                        {
                            Price = 99.5m,
                            ShippingAddress = "Some address 1",
                            ShippingDate = new DateTime(2019, 10, 1)
                        },
                        new Order
                        {
                            Price = 23,
                            ShippingAddress = "Some address 2",
                            ShippingDate = new DateTime(2019, 10, 10)
                        }
                    }
                };

                static Customer CreateCustomer2() => new Customer
                {
                    Name = "Moe",
                    Age = 35,
                    ID = Guid.Parse("3272b593-bfe2-4ecf-81ae-4242b0632465"),
                    IsVip = true,
                    Statistics = new Statistics
                    {
                        Visits = 20,
                        Purchases = 25,
                        Nested = new NestedStatistics
                        {
                            SomeProperty = 20,
                            SomeNullableInt = null,
                            SomeNullableGuid = null,
                            IntArray = new[] { 5, 6 }
                        }
                    },
                    Orders = new[]
                    {
                        new Order
                        {
                            Price = 5,
                            ShippingAddress = "Moe's address",
                            ShippingDate = new DateTime(2019, 11, 3)
                        }
                    }
                };
            }
        }

        public class JsonEntity
        {
            public int Id { get; set; }

            [Column(TypeName = "json")]
            public Customer Customer { get; set; }

            [Column(TypeName = "json")]
            public string[] ToplevelArray { get; set; }
        }

        public class JsonPocoQueryFixture : SharedStoreFixtureBase<JsonPocoQueryContext>
        {
            protected override string StoreName => "JsonPocoQueryTest";
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
            public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;
            protected override void Seed(JsonPocoQueryContext context) => JsonPocoQueryContext.Seed(context);

            protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
            {
                return base.AddServices(serviceCollection)
                    .AddEntityFrameworkMySqlJson(new MicrosoftJsonSerializer());
            }

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                var options = base.AddOptions(builder);
                new MySqlDbContextOptionsBuilder(options)
                    .UseMicrosoftJson();

                return options;
            }
        }

        public class Customer
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public Guid ID { get; set; }
            [JsonPropertyName("is_vip")]
            public bool IsVip { get; set; }
            public Statistics Statistics { get; set; }
            public Order[] Orders { get; set; }
        }

        public class Statistics
        {
            public long Visits { get; set; }
            public int Purchases { get; set; }
            public NestedStatistics Nested { get; set; }
        }

        public class NestedStatistics
        {
            public int SomeProperty { get; set; }
            public int? SomeNullableInt { get; set; }
            public int[] IntArray { get; set; }
            public Guid? SomeNullableGuid { get; set; }
        }

        public class Order
        {
            public decimal Price { get; set; }
            public string ShippingAddress { get; set; }
            public DateTime ShippingDate { get; set; }
        }

        #endregion
    }
}
