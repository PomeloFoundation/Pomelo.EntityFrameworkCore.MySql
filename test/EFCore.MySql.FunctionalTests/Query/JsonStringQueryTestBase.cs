using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public abstract class JsonStringQueryTestBase<TFixture> : IClassFixture<TFixture>
        where TFixture : JsonStringQueryTestBase<TFixture>.JsonStringQueryFixtureBase
    {
        protected JsonStringQueryFixtureBase Fixture { get; }

        protected JsonStringQueryTestBase(JsonStringQueryFixtureBase fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void Roundtrip()
        {
            using var ctx = CreateContext();
            var entity = ctx.JsonEntities.Single(e => e.Id == 1);
            PerformAsserts(entity.CustomerJson);
            PerformAsserts(entity.CustomerJson);

            static void PerformAsserts(string customerText)
            {
                var customer = JsonDocument.Parse(customerText).RootElement;

                Assert.Equal("Joe", customer.GetProperty("Name").GetString());
                Assert.Equal(25, customer.GetProperty("Age").GetInt32());

                var order1 = customer.GetProperty("Orders")[0];

                Assert.Equal(99.5m, order1.GetProperty("Price").GetDecimal());
                Assert.Equal("Some address 1", order1.GetProperty("ShippingAddress").GetString());
                Assert.Equal(new DateTime(2019, 10, 1), order1.GetProperty("ShippingDate").GetDateTime());

                var order2 = customer.GetProperty("Orders")[1];

                Assert.Equal(23, order2.GetProperty("Price").GetDecimal());
                Assert.Equal("Some address 2", order2.GetProperty("ShippingAddress").GetString());
                Assert.Equal(new DateTime(2019, 10, 10), order2.GetProperty("ShippingDate").GetDateTime());
            }
        }

        [Fact]
        public void Literal()
        {
            using var ctx = CreateContext();

            Assert.Empty(ctx.JsonEntities.Where(e => e.CustomerJson == @"{""Name"":""Test customer"",""Age"":80,""IsVip"":false,""Statistics"":null,""Orders"":null}"));
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJson`
FROM `JsonEntities` AS `j`
WHERE `j`.`CustomerJson` = '{""Name"":""Test customer"",""Age"":80,""IsVip"":false,""Statistics"":null,""Orders"":null}'");
        }

        [Fact]
        public void Parameter()
        {
            using var ctx = CreateContext();
            var expected = ctx.JsonEntities.Find(1).CustomerJson;
            var actual = ctx.JsonEntities.Single(e => e.CustomerJson == EF.Functions.AsJson(expected)).CustomerJson;

            Assert.Equal(actual, expected);
            AssertSql(
                @"@__p_0='1'

SELECT `j`.`Id`, `j`.`CustomerJson`
FROM `JsonEntities` AS `j`
WHERE `j`.`Id` = @__p_0
LIMIT 1",
                //
                $@"{InsertJsonDocument(@"@__expected_1='{""Age"":25,""Name"":""Joe"",""IsVip"":false,""Orders"":[{""Price"":99.5,""ShippingDate"":""2019-10-01"",""ShippingAddress"":""Some address 1""},{""Price"":23,""ShippingDate"":""2019-10-10"",""ShippingAddress"":""Some address 2""}],""Statistics"":{""Nested"":{""IntArray"":[3,4],""SomeProperty"":10},""Visits"":4,""Purchases"":3}}'", @"@__expected_1='{""Name"":""Joe"",""Age"":25,""IsVip"":false,""Statistics"":{""Visits"":4,""Purchases"":3,""Nested"":{""SomeProperty"":10,""IntArray"":[3,4]}},""Orders"":[{""Price"":99.5,""ShippingAddress"":""Some address 1"",""ShippingDate"":""2019-10-01""},{""Price"":23,""ShippingAddress"":""Some address 2"",""ShippingDate"":""2019-10-10""}]}'")} (Size = 4000)

SELECT `j`.`Id`, `j`.`CustomerJson`
FROM `JsonEntities` AS `j`
WHERE {InsertJsonConvert("`j`.`CustomerJson`")} = {InsertJsonConvert("@__expected_1")}
LIMIT 2");
        }

        [Fact]
        public void Parameter_cast_MySqlJsonString()
        {
            using var ctx = CreateContext();
            var expected = ctx.JsonEntities.Find(1).CustomerJson;
            var actual = ctx.JsonEntities.Single(e => e.CustomerJson == (MySqlJsonString)expected).CustomerJson;

            Assert.Equal(actual, expected);
            AssertSql(
                @"@__p_0='1'

SELECT `j`.`Id`, `j`.`CustomerJson`
FROM `JsonEntities` AS `j`
WHERE `j`.`Id` = @__p_0
LIMIT 1",
                //
                $@"{InsertJsonDocument(@"@__p_0='{""Age"":25,""Name"":""Joe"",""IsVip"":false,""Orders"":[{""Price"":99.5,""ShippingDate"":""2019-10-01"",""ShippingAddress"":""Some address 1""},{""Price"":23,""ShippingDate"":""2019-10-10"",""ShippingAddress"":""Some address 2""}],""Statistics"":{""Nested"":{""IntArray"":[3,4],""SomeProperty"":10},""Visits"":4,""Purchases"":3}}'", @"@__p_0='{""Name"":""Joe"",""Age"":25,""IsVip"":false,""Statistics"":{""Visits"":4,""Purchases"":3,""Nested"":{""SomeProperty"":10,""IntArray"":[3,4]}},""Orders"":[{""Price"":99.5,""ShippingAddress"":""Some address 1"",""ShippingDate"":""2019-10-01""},{""Price"":23,""ShippingAddress"":""Some address 2"",""ShippingDate"":""2019-10-10""}]}'")} (Size = 4000)

SELECT `j`.`Id`, `j`.`CustomerJson`
FROM `JsonEntities` AS `j`
WHERE {InsertJsonConvert("`j`.`CustomerJson`")} = {InsertJsonConvert("@__p_0")}
LIMIT 2");
        }

        #region Functions

        [Fact]
        public void JsonExtract()
        {
            using var ctx = CreateContext();

            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonExtract<string>(e.CustomerJson, "$.Name") == @"Joe");

            Assert.Equal(1, count);
            AssertSql(
                $@"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerJson`, '$.Name') = 'Joe'");
        }

        [Fact]
        public void JsonExtract_JsonUnquote()
        {
            using var ctx = CreateContext();

            var name = @"Joe";
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonUnquote(EF.Functions.JsonExtract<string>(e.CustomerJson, "$.Name")) == name);

            Assert.Equal(1, count);
            AssertSql(
                $@"@__name_1='Joe' (Size = 4000)

SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_UNQUOTE(JSON_EXTRACT(`j`.`CustomerJson`, '$.Name')) = @__name_1");
        }

        [Fact]
        public void JsonExtract_JsonUnquote_JsonQuote()
        {
            using var ctx = CreateContext();

            var name = @"""Joe""";
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonQuote(EF.Functions.JsonUnquote(EF.Functions.JsonExtract<string>(e.CustomerJson, "$.Name"))) == name);

            Assert.Equal(1, count);
            AssertSql(
                $@"@__name_1='""Joe""' (Size = 4000)

SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_QUOTE(JSON_UNQUOTE(JSON_EXTRACT(`j`.`CustomerJson`, '$.Name'))) = @__name_1");
        }

        [Fact]
        public void JsonContains_with_string()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContains(e.CustomerJson, @"{""Name"": ""Joe"", ""Age"": 25}"));

            Assert.Equal(1, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS(`j`.`CustomerJson`, '{""Name"": ""Joe"", ""Age"": 25}')");
        }

        [Fact]
        public void JsonExists()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContainsPath(e.CustomerJson, "$.Age"));

            Assert.Equal(2, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS_PATH(`j`.`CustomerJson`, 'one', '$.Age')");
        }

        [Fact]
        public void JsonExistAny()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContainsPathAny(e.CustomerJson, "$.foo", "$.Age"));

            Assert.Equal(2, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS_PATH(`j`.`CustomerJson`, 'one', '$.foo', '$.Age')");
        }

        [Fact]
        public void JsonExistAll()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContainsPathAll(e.CustomerJson, "$.foo", "$.Age"));

            Assert.Equal(0, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS_PATH(`j`.`CustomerJson`, 'all', '$.foo', '$.Age')");
        }

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

        protected JsonStringQueryContext CreateContext() => Fixture.CreateContext();

        void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class JsonStringQueryContext : PoolableDbContext
        {
            public DbSet<JsonEntity> JsonEntities { get; set; }

            public JsonStringQueryContext(DbContextOptions options) : base(options) {}

            public static void Seed(JsonStringQueryContext context)
            {
                const string customer1 = @"
                {
                    ""Name"": ""Joe"",
                    ""Age"": 25,
                    ""IsVip"": false,
                    ""Statistics"":
                    {
                        ""Visits"": 4,
                        ""Purchases"": 3,
                        ""Nested"":
                        {
                            ""SomeProperty"": 10,
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
                            ""Price"": 23,
                            ""ShippingAddress"": ""Some address 2"",
                            ""ShippingDate"": ""2019-10-10""
                        }
                    ]
                }";

                const string customer2 = @"
                {
                    ""Name"": ""Moe"",
                    ""Age"": 35,
                    ""IsVip"": true,
                    ""Statistics"":
                    {
                        ""Visits"": 20,
                        ""Purchases"": 25,
                        ""Nested"":
                        {
                            ""SomeProperty"": 20,
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
                }";

                context.JsonEntities.AddRange(
                    new JsonEntity { Id = 1, CustomerJson = customer1 },
                    new JsonEntity { Id = 2, CustomerJson = customer2 });
                context.SaveChanges();
            }
        }

        public class JsonEntity
        {
            public int Id { get; set; }

            [Column(TypeName = "json")]
            public string CustomerJson { get; set; }
        }

        public class JsonStringQueryFixtureBase : SharedStoreFixtureBase<JsonStringQueryContext>
        {
            protected override string StoreName => "JsonStringQueryTest";
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
            public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;
            protected override void Seed(JsonStringQueryContext context) => JsonStringQueryContext.Seed(context);
        }

        #endregion
    }
}
