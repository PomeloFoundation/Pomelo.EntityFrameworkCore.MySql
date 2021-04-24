using System;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.Json))]
    public class JsonMicrosoftDomQueryTest : IClassFixture<JsonMicrosoftDomQueryTest.JsonMicrosoftDomQueryFixture>
    {
        protected JsonMicrosoftDomQueryFixture Fixture { get; }

        public JsonMicrosoftDomQueryTest(JsonMicrosoftDomQueryFixture fixture, ITestOutputHelper testOutputHelper)
        {
            Fixture = fixture;
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [Fact]
        public void Roundtrip()
        {
            using var ctx = CreateContext();

            var json = ctx.JsonEntities.Single(e => e.Id == 1);
            performAsserts(json.CustomerDocument.RootElement);
            performAsserts(json.CustomerElement);

            static void performAsserts(JsonElement customer)
            {
                Assert.Equal("Joe", customer.GetProperty("Name").GetString());
                Assert.Equal(25, customer.GetProperty("Age").GetInt32());

                var order1 = customer.GetProperty("Orders")[0];

                Assert.Equal(99.5m, order1.GetProperty("Price").GetDecimal());
                Assert.Equal("Some address 1", order1.GetProperty("ShippingAddress").GetString());
                Assert.Equal(new DateTime(2019, 10, 1), order1.GetProperty("ShippingDate").GetDateTime());

                var order2 = customer.GetProperty("Orders")[1];

                Assert.Equal(23.1m, order2.GetProperty("Price").GetDecimal());
                Assert.Equal("Some address 2", order2.GetProperty("ShippingAddress").GetString());
                Assert.Equal(new DateTime(2019, 10, 10), order2.GetProperty("ShippingDate").GetDateTime());
            }
        }

        [Fact]
        public void Literal_document()
        {
            using var ctx = CreateContext();

            Assert.Empty(ctx.JsonEntities.Where(e => e.CustomerDocument == JsonDocument.Parse(@"
{ ""Name"": ""Test customer"", ""Age"": 80 }", default)));
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE `j`.`CustomerDocument` = '{""Name"":""Test customer"",""Age"":80}'");
        }

        [Fact]
        public void Parameter_document()
        {
            using var ctx = CreateContext();
            var expected = ctx.JsonEntities.Find(1).CustomerDocument;
            var actual = ctx.JsonEntities.Single(e => e.CustomerDocument == expected).CustomerDocument;

            Assert.Equal(actual, expected);
            AssertSql(
                @"@__p_0='1'

SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE `j`.`Id` = @__p_0
LIMIT 1",
                //
                $@"{InsertJsonDocument(@"@__expected_0='{""ID"":""00000000-0000-0000-0000-000000000000"",""Age"":25,""Name"":""Joe"",""IsVip"":false,""Orders"":[{""Price"":99.5,""ShippingDate"":""2019-10-01"",""ShippingAddress"":""Some address 1""},{""Price"":23.1,""ShippingDate"":""2019-10-10"",""ShippingAddress"":""Some address 2""}],""Statistics"":{""Nested"":{""IntArray"":[3,4],""SomeProperty"":10,""SomeNullableInt"":20,""SomeNullableGuid"":""d5f2685d-e5c4-47e5-97aa-d0266154eb2d""},""Visits"":4,""Purchases"":3}}'", @"@__expected_0='{""Name"":""Joe"",""Age"":25,""ID"":""00000000-0000-0000-0000-000000000000"",""IsVip"":false,""Statistics"":{""Visits"":4,""Purchases"":3,""Nested"":{""SomeProperty"":10,""SomeNullableInt"":20,""SomeNullableGuid"":""d5f2685d-e5c4-47e5-97aa-d0266154eb2d"",""IntArray"":[3,4]}},""Orders"":[{""Price"":99.5,""ShippingAddress"":""Some address 1"",""ShippingDate"":""2019-10-01""},{""Price"":23.1,""ShippingAddress"":""Some address 2"",""ShippingDate"":""2019-10-10""}]}'")} (Size = 4000)

SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE `j`.`CustomerDocument` = {InsertJsonConvert("@__expected_0")}
LIMIT 2");
        }

        [Fact]
        public void Parameter_element()
        {
            using var ctx = CreateContext();
            var expected = ctx.JsonEntities.Find(1).CustomerElement;
            var actual = ctx.JsonEntities.Single(e => e.CustomerElement.Equals(expected)).CustomerElement;

            Assert.Equal(actual, expected);
            AssertSql(
                @"@__p_0='1'

SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE `j`.`Id` = @__p_0
LIMIT 1",
                //
                $@"{InsertJsonDocument(@"@__expected_0='{""ID"":""00000000-0000-0000-0000-000000000000"",""Age"":25,""Name"":""Joe"",""IsVip"":false,""Orders"":[{""Price"":99.5,""ShippingDate"":""2019-10-01"",""ShippingAddress"":""Some address 1""},{""Price"":23.1,""ShippingDate"":""2019-10-10"",""ShippingAddress"":""Some address 2""}],""Statistics"":{""Nested"":{""IntArray"":[3,4],""SomeProperty"":10,""SomeNullableInt"":20,""SomeNullableGuid"":""d5f2685d-e5c4-47e5-97aa-d0266154eb2d""},""Visits"":4,""Purchases"":3}}'", @"@__expected_0='{""Name"":""Joe"",""Age"":25,""ID"":""00000000-0000-0000-0000-000000000000"",""IsVip"":false,""Statistics"":{""Visits"":4,""Purchases"":3,""Nested"":{""SomeProperty"":10,""SomeNullableInt"":20,""SomeNullableGuid"":""d5f2685d-e5c4-47e5-97aa-d0266154eb2d"",""IntArray"":[3,4]}},""Orders"":[{""Price"":99.5,""ShippingAddress"":""Some address 1"",""ShippingDate"":""2019-10-01""},{""Price"":23.1,""ShippingAddress"":""Some address 2"",""ShippingDate"":""2019-10-10""}]}'")} (Nullable = false) (Size = 4000)

SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE `j`.`CustomerElement` = {InsertJsonConvert("@__expected_0")}
LIMIT 2");
        }

        [Fact]
        public void Text_output_on_document()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerDocument.RootElement.GetProperty("Name").GetString() == "Joe");

            Assert.Equal("Joe", x.CustomerElement.GetProperty("Name").GetString());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE JSON_UNQUOTE(JSON_EXTRACT(`j`.`CustomerDocument`, '$.Name')) = 'Joe'
LIMIT 2");
        }

        [Fact]
        public void Text_output()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerElement.GetProperty("Name").GetString() == "Joe");

            Assert.Equal("Joe", x.CustomerElement.GetProperty("Name").GetString());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE JSON_UNQUOTE(JSON_EXTRACT(`j`.`CustomerElement`, '$.Name')) = 'Joe'
LIMIT 2");
        }

        [Fact]
        public void Integer_output()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerElement.GetProperty("Age").GetInt32() < 30);

            Assert.Equal("Joe", x.CustomerElement.GetProperty("Name").GetString());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerElement`, '$.Age') < 30
LIMIT 2");
        }

        [Fact]
        public void Guid_output()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerElement.GetProperty("ID").GetGuid() == Guid.Empty);

            Assert.Equal("Joe", x.CustomerElement.GetProperty("Name").GetString());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerElement`, '$.ID') = '00000000-0000-0000-0000-000000000000'
LIMIT 2");
        }

        [Fact]
        public void Bool_output()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerElement.GetProperty("IsVip").GetBoolean());

            Assert.Equal("Moe", x.CustomerElement.GetProperty("Name").GetString());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerElement`, '$.IsVip') = TRUE
LIMIT 2");
        }

        [Fact]
        public void Nested()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerElement.GetProperty("Statistics").GetProperty("Visits").GetInt64() == 4);

            Assert.Equal("Joe", x.CustomerElement.GetProperty("Name").GetString());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerElement`, '$.Statistics.Visits') = 4
LIMIT 2");
        }

        [Fact]
        public void Nested_twice()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerElement.GetProperty("Statistics").GetProperty("Nested").GetProperty("SomeProperty").GetInt32() == 10);

            Assert.Equal("Joe", x.CustomerElement.GetProperty("Name").GetString());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerElement`, '$.Statistics.Nested.SomeProperty') = 10
LIMIT 2");
        }

        [Fact]
        public void Array_of_objects()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerElement.GetProperty("Orders")[0].GetProperty("Price").GetDecimal() == 99.5m);

            Assert.Equal("Joe", x.CustomerElement.GetProperty("Name").GetString());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerElement`, '$.Orders[0].Price') = 99.5
LIMIT 2");
        }

        [Fact]
        public void Array_nested()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e =>
                e.CustomerElement.GetProperty("Statistics").GetProperty("Nested").GetProperty("IntArray")[1].GetInt32() == 4);

            Assert.Equal("Joe", x.CustomerElement.GetProperty("Name").GetString());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerElement`, '$.Statistics.Nested.IntArray[1]') = 4
LIMIT 2");
        }

        [Fact]
        public void Array_parameter_index()
        {
            using var ctx = CreateContext();
            var i = 1;
            var x = ctx.JsonEntities.Single(e =>
                e.CustomerElement.GetProperty("Statistics").GetProperty("Nested").GetProperty("IntArray")[i].GetInt32() == 4);

            Assert.Equal("Joe", x.CustomerElement.GetProperty("Name").GetString());
            AssertSql(
                @"@__i_0='1'

SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerElement`, CONCAT('$.Statistics.Nested.IntArray[', @__i_0, ']')) = 4
LIMIT 2");
        }

        [Fact]
        public void GetArrayLength()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerElement.GetProperty("Orders").GetArrayLength() == 2);

            Assert.Equal("Joe", x.CustomerElement.GetProperty("Name").GetString());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE JSON_LENGTH(JSON_EXTRACT(`j`.`CustomerElement`, '$.Orders')) = 2
LIMIT 2");
        }

        [Fact]
        public void Like()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerElement.GetProperty("Name").GetString().StartsWith("J"));

            Assert.Equal("Joe", x.CustomerElement.GetProperty("Name").GetString());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE JSON_UNQUOTE(JSON_EXTRACT(`j`.`CustomerElement`, '$.Name')) LIKE 'J%'
LIMIT 2");
        }

        [Fact]
        public void Where_nullable_guid()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e =>
                e.CustomerElement.GetProperty("Statistics").GetProperty("Nested").GetProperty("SomeNullableGuid").GetGuid()
                == Guid.Parse("d5f2685d-e5c4-47e5-97aa-d0266154eb2d"));

            Assert.Equal("Joe", x.CustomerElement.GetProperty("Name").GetString());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerElement`, '$.Statistics.Nested.SomeNullableGuid') = 'd5f2685d-e5c4-47e5-97aa-d0266154eb2d'
LIMIT 2");
        }

        [Fact]
        public void Where_root_value()
        {
            using var ctx = CreateContext();
            _ = ctx.JsonEntities.Single(e => e.CustomerElement.GetString() == "foo");

            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE JSON_UNQUOTE(`j`.`CustomerElement`) = 'foo'
LIMIT 2");
        }

        #region Functions

        [Fact]
        public void JsonContains_with_json_element()
        {
            using var ctx = CreateContext();
            var element = JsonDocument.Parse(@"{""Name"": ""Joe"", ""Age"": 25}").RootElement;
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContains(e.CustomerElement, element));

            Assert.Equal(1, count);
            AssertSql(
                $@"@__element_1='{{""Name"":""Joe"",""Age"":25}}' (Nullable = false) (Size = 4000)

SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS(`j`.`CustomerElement`, {InsertJsonConvert("@__element_1")})");
        }

        [Fact]
        public void JsonContains_with_string()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContains(e.CustomerElement, @"{""Name"": ""Joe"", ""Age"": 25}"));

            Assert.Equal(1, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS(`j`.`CustomerElement`, '{""Name"": ""Joe"", ""Age"": 25}')");
        }

        [Fact]
        public void JsonContainsPath()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContainsPath(e.CustomerElement, "$.Statistics.Visits"));

            Assert.Equal(2, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS_PATH(`j`.`CustomerElement`, 'one', '$.Statistics.Visits')");
        }

        [Fact]
        public void JsonContainsPathAny()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContainsPathAny(e.CustomerElement, "$.Statistics.Foo", "$.Statistics.Visits"));

            Assert.Equal(2, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS_PATH(`j`.`CustomerElement`, 'one', '$.Statistics.Foo', '$.Statistics.Visits')");
        }

        [Fact]
        public void JsonContainsPathAll()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContainsPathAll(e.CustomerElement, "$.Statistics.Foo", "$.Statistics.Visits"));

            Assert.Equal(0, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS_PATH(`j`.`CustomerElement`, 'all', '$.Statistics.Foo', '$.Statistics.Visits')");
        }

        [Fact]
        public void JsonType()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonType(e.CustomerElement.GetProperty("Statistics").GetProperty("Visits")) == "INTEGER");

            Assert.Equal(2, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_TYPE(JSON_EXTRACT(`j`.`CustomerElement`, '$.Statistics.Visits')) = 'INTEGER'");
        }

        [Fact]
        public void JsonSearchAny()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => EF.Functions.JsonSearchAny(e.CustomerElement, "J%", "$.Name"));

            Assert.Equal("Joe", x.CustomerElement.GetProperty("Name").GetString());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerDocument`, `j`.`CustomerElement`
FROM `JsonEntities` AS `j`
WHERE JSON_SEARCH(`j`.`CustomerElement`, 'one', 'J%', NULL, '$.Name') IS NOT NULL
LIMIT 2");
        }

//         [Fact]
//         public void JsonSearchAll()
//         {
//             using var ctx = CreateContext();
//             var count = ctx.JsonEntities.Count(e => EF.Functions.JsonSearchAll(e.CustomerElement, "%o%"));
//
//             Assert.Equal(3, count);
//             AssertSql(
//                 $@"SELECT COUNT(*)
// FROM `JsonEntities` AS `j`
// WHERE JSON_SEARCH(`j`.`CustomerElement`, 'all', '%o%') IS NOT NULL");
//         }
//
//         [Fact]
//         public void JsonSearchAll_with_path()
//         {
//             using var ctx = CreateContext();
//             var count = ctx.JsonEntities.Count(e => EF.Functions.JsonSearchAll(e.CustomerElement, "%o%", "$.Name"));
//
//             Assert.Equal(2, count);
//             AssertSql(
//                 @"SELECT COUNT(*)
// FROM `JsonEntities` AS `j`
// WHERE JSON_SEARCH(`j`.`CustomerElement`, 'all', '%o%', NULL, '$.Name') IS NOT NULL");
//         }

        #endregion Functions

        #region Support

        protected string InsertJsonConvert(string sqlFragment)
            => AppConfig.ServerVersion.Supports.JsonDataTypeEmulation
                ? sqlFragment
                : $"CAST({sqlFragment} AS json)";

        protected string InsertJsonDocument(string mySqlDocument, string mariaDbDocument)
            => AppConfig.ServerVersion.Supports.JsonDataTypeEmulation
                ? mariaDbDocument
                : mySqlDocument;

        protected string InsertJsonImplementation(string mySqlSqlFragment = null, string mariaDbSqlFragment = null)
            => AppConfig.ServerVersion.Supports.JsonDataTypeEmulation
                ? mariaDbSqlFragment
                : mySqlSqlFragment;

        protected JsonDomQueryContext CreateContext() => Fixture.CreateContext();

        void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class JsonDomQueryContext : PoolableDbContext
        {
            public DbSet<JsonEntity> JsonEntities { get; set; }

            public JsonDomQueryContext(DbContextOptions options) : base(options) {}

            public static void Seed(JsonDomQueryContext context)
            {
                var (customer1, customer2, customer3) = (createCustomer1(), createCustomer2(), createCustomer3());

                context.JsonEntities.AddRange(
                    new JsonEntity { Id = 1, CustomerDocument = customer1, CustomerElement = customer1.RootElement },
                    new JsonEntity { Id = 2, CustomerDocument = customer2, CustomerElement = customer2.RootElement },
                    new JsonEntity { Id = 3, CustomerDocument = customer3, CustomerElement = customer3.RootElement });
                context.SaveChanges();

                static JsonDocument createCustomer1() => JsonDocument.Parse(@"
                {
                    ""Name"": ""Joe"",
                    ""Age"": 25,
                    ""ID"": ""00000000-0000-0000-0000-000000000000"",
                    ""IsVip"": false,
                    ""Statistics"":
                    {
                        ""Visits"": 4,
                        ""Purchases"": 3,
                        ""Nested"":
                        {
                            ""SomeProperty"": 10,
                            ""SomeNullableInt"": 20,
                            ""SomeNullableGuid"": ""d5f2685d-e5c4-47e5-97aa-d0266154eb2d"",
                            ""IntArray"": [3, 4]
                        }
                    },
                    ""Orders"":
                    [
                        {
                            ""Price"": 99.5,
                            ""ShippingAddress"": ""Some address 1"",
                            ""ShippingDate"": ""2019-10-01""
                        },
                        {
                            ""Price"": 23.1,
                            ""ShippingAddress"": ""Some address 2"",
                            ""ShippingDate"": ""2019-10-10""
                        }
                    ]
                }");

                static JsonDocument createCustomer2() => JsonDocument.Parse(@"
                {
                    ""Name"": ""Moe"",
                    ""Age"": 35,
                    ""ID"": ""3272b593-bfe2-4ecf-81ae-4242b0632465"",
                    ""IsVip"": true,
                    ""Statistics"":
                    {
                        ""Visits"": 20,
                        ""Purchases"": 25,
                        ""Nested"":
                        {
                            ""SomeProperty"": 20,
                            ""SomeNullableInt"": null,
                            ""SomeNullableGuid"": null,
                            ""IntArray"": [5, 6]
                        }
                    },
                    ""Orders"":
                    [
                        {
                            ""Price"": 5,
                            ""ShippingAddress"": ""Moe's address"",
                            ""ShippingDate"": ""2019-11-03""
                        }
                    ]
                }");

                static JsonDocument createCustomer3() => JsonDocument.Parse(@"""foo""");
            }
        }

        public class JsonEntity
        {
            public int Id { get; set; }

            public JsonDocument CustomerDocument { get; set; }
            public JsonElement CustomerElement { get; set; }
        }

        public class JsonMicrosoftDomQueryFixture : SharedStoreFixtureBase<JsonDomQueryContext>
        {
            protected override string StoreName => "JsonMicrosoftDomQueryTest";
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
            public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;
            protected override void Seed(JsonDomQueryContext context) => JsonDomQueryContext.Seed(context);

            protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
            {
                return base.AddServices(serviceCollection)
                    .AddEntityFrameworkMySqlJsonMicrosoft();
            }

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                var options = base.AddOptions(builder);
                new MySqlDbContextOptionsBuilder(options)
                    .UseMicrosoftJson();

                return options;
            }
        }

        #endregion
    }
}
