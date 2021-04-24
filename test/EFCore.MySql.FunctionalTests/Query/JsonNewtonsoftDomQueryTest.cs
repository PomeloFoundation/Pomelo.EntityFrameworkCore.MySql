using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.Json))]
    public class JsonNewtonsoftDomQueryTest : IClassFixture<JsonNewtonsoftDomQueryTest.JsonNewtonsoftDomQueryFixture>
    {
        protected JsonNewtonsoftDomQueryFixture Fixture { get; }

        public JsonNewtonsoftDomQueryTest(JsonNewtonsoftDomQueryFixture fixture, ITestOutputHelper testOutputHelper)
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
             performAsserts(json.CustomerJObject.Root);
             performAsserts(json.CustomerJToken);

             static void performAsserts(JToken customer)
             {
                 Assert.Equal("Joe", customer["Name"].Value<string>());
                 Assert.Equal(25, customer["Age"].Value<int>());

                 var order1 = customer["Orders"][0];

                 Assert.Equal(99.5m, order1["Price"].Value<decimal>());
                 Assert.Equal("Some address 1", order1["ShippingAddress"].Value<string>());
                 Assert.Equal(new DateTime(2019, 10, 1), order1["ShippingDate"].Value<DateTime>());

                 var order2 = customer["Orders"][1];

                 Assert.Equal(23.1m, order2["Price"].Value<decimal>());
                 Assert.Equal("Some address 2", order2["ShippingAddress"].Value<string>());
                 Assert.Equal(new DateTime(2019, 10, 10), order2["ShippingDate"].Value<DateTime>());
             }
         }

        [Fact]
        public void Literal_document()
        {
            using var ctx = CreateContext();

            Assert.Empty(ctx.JsonEntities.Where(e => e.CustomerJObject == JObject.Parse(@"
{ ""Name"": ""Test customer"", ""Age"": 80 }", default)));
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE `j`.`CustomerJObject` = '{""Name"":""Test customer"",""Age"":80}'");
        }

        [Fact]
        public void Parameter_document()
        {
            using var ctx = CreateContext();
            var expected = ctx.JsonEntities.Find(1).CustomerJObject;
            var actual = ctx.JsonEntities.Single(e => e.CustomerJObject == expected).CustomerJObject;

            Assert.Equal(actual, expected);
            AssertSql(
                @"@__p_0='1'

SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE `j`.`Id` = @__p_0
LIMIT 1",
                //
                $@"{InsertJsonDocument(@"@__expected_0='{""ID"":""00000000-0000-0000-0000-000000000000"",""Age"":25,""Name"":""Joe"",""IsVip"":false,""Orders"":[{""Price"":99.5,""ShippingDate"":""2019-10-01"",""ShippingAddress"":""Some address 1""},{""Price"":23.1,""ShippingDate"":""2019-10-10"",""ShippingAddress"":""Some address 2""}],""Statistics"":{""Nested"":{""IntArray"":[3,4],""SomeProperty"":10,""SomeNullableInt"":20,""SomeNullableGuid"":""d5f2685d-e5c4-47e5-97aa-d0266154eb2d""},""Visits"":4,""Purchases"":3}}'", @"@__expected_0='{""Name"":""Joe"",""Age"":25,""ID"":""00000000-0000-0000-0000-000000000000"",""IsVip"":false,""Statistics"":{""Visits"":4,""Purchases"":3,""Nested"":{""SomeProperty"":10,""SomeNullableInt"":20,""SomeNullableGuid"":""d5f2685d-e5c4-47e5-97aa-d0266154eb2d"",""IntArray"":[3,4]}},""Orders"":[{""Price"":99.5,""ShippingAddress"":""Some address 1"",""ShippingDate"":""2019-10-01""},{""Price"":23.1,""ShippingAddress"":""Some address 2"",""ShippingDate"":""2019-10-10""}]}'")} (Size = 4000)

SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE `j`.`CustomerJObject` = {InsertJsonConvert("@__expected_0")}
LIMIT 2");
        }

        [Fact]
        public void Parameter_element()
        {
            using var ctx = CreateContext();
            var expected = ctx.JsonEntities.Find(1).CustomerJToken;
            var actual = ctx.JsonEntities.Single(e => e.CustomerJToken.Equals(expected)).CustomerJToken;

            Assert.Equal(actual, expected);
            AssertSql(
                @"@__p_0='1'

SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE `j`.`Id` = @__p_0
LIMIT 1",
                //
                $@"{InsertJsonDocument(@"@__expected_0='{""ID"":""00000000-0000-0000-0000-000000000000"",""Age"":25,""Name"":""Joe"",""IsVip"":false,""Orders"":[{""Price"":99.5,""ShippingDate"":""2019-10-01"",""ShippingAddress"":""Some address 1""},{""Price"":23.1,""ShippingDate"":""2019-10-10"",""ShippingAddress"":""Some address 2""}],""Statistics"":{""Nested"":{""IntArray"":[3,4],""SomeProperty"":10,""SomeNullableInt"":20,""SomeNullableGuid"":""d5f2685d-e5c4-47e5-97aa-d0266154eb2d""},""Visits"":4,""Purchases"":3}}'", @"@__expected_0='{""Name"":""Joe"",""Age"":25,""ID"":""00000000-0000-0000-0000-000000000000"",""IsVip"":false,""Statistics"":{""Visits"":4,""Purchases"":3,""Nested"":{""SomeProperty"":10,""SomeNullableInt"":20,""SomeNullableGuid"":""d5f2685d-e5c4-47e5-97aa-d0266154eb2d"",""IntArray"":[3,4]}},""Orders"":[{""Price"":99.5,""ShippingAddress"":""Some address 1"",""ShippingDate"":""2019-10-01""},{""Price"":23.1,""ShippingAddress"":""Some address 2"",""ShippingDate"":""2019-10-10""}]}'")} (Size = 4000)

SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE `j`.`CustomerJToken` = {InsertJsonConvert("@__expected_0")}
LIMIT 2");
        }

        [Fact]
        public void Text_output_on_document()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerJObject.Root["Name"].Value<string>() == "Joe");

            Assert.Equal("Joe", x.CustomerJToken["Name"].Value<string>());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE JSON_UNQUOTE(JSON_EXTRACT(`j`.`CustomerJObject`, '$.Name')) = 'Joe'
LIMIT 2");
        }

        [Fact]
        public void Text_output()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerJToken["Name"].Value<string>() == "Joe");

            Assert.Equal("Joe", x.CustomerJToken["Name"].Value<string>());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE JSON_UNQUOTE(JSON_EXTRACT(`j`.`CustomerJToken`, '$.Name')) = 'Joe'
LIMIT 2");
        }

        [Fact]
        public void Integer_output()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerJToken["Age"].Value<int>() < 30);

            Assert.Equal("Joe", x.CustomerJToken["Name"].Value<string>());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerJToken`, '$.Age') < 30
LIMIT 2");
        }

        [Fact]
        public void Guid_output()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerJToken["ID"].Value<Guid>() == Guid.Empty);

            Assert.Equal("Joe", x.CustomerJToken["Name"].Value<string>());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerJToken`, '$.ID') = '00000000-0000-0000-0000-000000000000'
LIMIT 2");
        }

        [Fact]
        public void Bool_output()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerJToken["IsVip"].Value<bool>());

            Assert.Equal("Moe", x.CustomerJToken["Name"].Value<string>());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerJToken`, '$.IsVip') = TRUE
LIMIT 2");
        }

        [Fact]
        public void Nested()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerJToken["Statistics"]["Visits"].Value<long>() == 4);

            Assert.Equal("Joe", x.CustomerJToken["Name"].Value<string>());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerJToken`, '$.Statistics.Visits') = 4
LIMIT 2");
        }

        [Fact]
        public void Nested_twice()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerJToken["Statistics"]["Nested"]["SomeProperty"].Value<int>() == 10);

            Assert.Equal("Joe", x.CustomerJToken["Name"].Value<string>());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerJToken`, '$.Statistics.Nested.SomeProperty') = 10
LIMIT 2");
        }

        [Fact]
        public void Array_of_objects()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerJToken["Orders"][0]["Price"].Value<decimal>() == 99.5m);

            Assert.Equal("Joe", x.CustomerJToken["Name"].Value<string>());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerJToken`, '$.Orders[0].Price') = 99.5
LIMIT 2");
        }

        [Fact]
        public void Array_nested()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e =>
                e.CustomerJToken["Statistics"]["Nested"]["IntArray"][1].Value<int>() == 4);

            Assert.Equal("Joe", x.CustomerJToken["Name"].Value<string>());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerJToken`, '$.Statistics.Nested.IntArray[1]') = 4
LIMIT 2");
        }

        [Fact]
        public void Array_parameter_index()
        {
            using var ctx = CreateContext();
            var i = 1;
            var x = ctx.JsonEntities.Single(e =>
                e.CustomerJToken["Statistics"]["Nested"]["IntArray"][i].Value<int>() == 4);

            Assert.Equal("Joe", x.CustomerJToken["Name"].Value<string>());
            AssertSql(
                @"@__i_0='1'

SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerJToken`, CONCAT('$.Statistics.Nested.IntArray[', @__i_0, ']')) = 4
LIMIT 2");
        }

        [Fact]
        public void GetArrayLength_Enumerable_Count()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerJToken["Orders"].Count() == 2);

            Assert.Equal("Joe", x.CustomerJToken["Name"].Value<string>());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE JSON_LENGTH(JSON_EXTRACT(`j`.`CustomerJToken`, '$.Orders')) = 2
LIMIT 2");
        }

        [Fact]
        public void GetArrayLength_JContainer_Count()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => ((JContainer)e.CustomerJToken["Orders"]).Count == 2);

            Assert.Equal("Joe", x.CustomerJToken["Name"].Value<string>());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE JSON_LENGTH(JSON_EXTRACT(`j`.`CustomerJToken`, '$.Orders')) = 2
LIMIT 2");
        }

        [Fact]
        public void GetArrayLength_JArray_Count()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => ((JArray)e.CustomerJToken["Orders"]).Count == 2);

            Assert.Equal("Joe", x.CustomerJToken["Name"].Value<string>());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE JSON_LENGTH(JSON_EXTRACT(`j`.`CustomerJToken`, '$.Orders')) = 2
LIMIT 2");
        }

        [Fact]
        public void Like()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => e.CustomerJToken["Name"].Value<string>().StartsWith("J"));

            Assert.Equal("Joe", x.CustomerJToken["Name"].Value<string>());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE JSON_UNQUOTE(JSON_EXTRACT(`j`.`CustomerJToken`, '$.Name')) LIKE 'J%'
LIMIT 2");
        }

        [Fact]
        public void Where_nullable_guid()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e =>
                e.CustomerJToken["Statistics"]["Nested"]["SomeNullableGuid"].Value<Guid>()
                == Guid.Parse("d5f2685d-e5c4-47e5-97aa-d0266154eb2d"));

            Assert.Equal("Joe", x.CustomerJToken["Name"].Value<string>());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE JSON_EXTRACT(`j`.`CustomerJToken`, '$.Statistics.Nested.SomeNullableGuid') = 'd5f2685d-e5c4-47e5-97aa-d0266154eb2d'
LIMIT 2");
        }

        [Fact]
        public void Where_root_value()
        {
            using var ctx = CreateContext();
            _ = ctx.JsonEntities.Single(e => e.CustomerJToken.Value<string>() == "foo");

            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE JSON_UNQUOTE(`j`.`CustomerJToken`) = 'foo'
LIMIT 2");
        }

         #region Functions

        [Fact]
        public void JsonContains_with_json_element()
        {
            using var ctx = CreateContext();
            var element = JObject.Parse(@"{""Name"": ""Joe"", ""Age"": 25}").Root;
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContains(e.CustomerJToken, element));

            Assert.Equal(1, count);
            AssertSql(
                $@"@__element_1='{{""Name"":""Joe"",""Age"":25}}' (Size = 4000)

SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS(`j`.`CustomerJToken`, {InsertJsonConvert("@__element_1")})");
        }

        [Fact]
        public void JsonContains_with_string()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContains(e.CustomerJToken, @"{""Name"": ""Joe"", ""Age"": 25}"));

            Assert.Equal(1, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS(`j`.`CustomerJToken`, '{""Name"": ""Joe"", ""Age"": 25}')");
        }

        [Fact]
        public void JsonContainsPath()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContainsPath(e.CustomerJToken, "$.Statistics.Visits"));

            Assert.Equal(2, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS_PATH(`j`.`CustomerJToken`, 'one', '$.Statistics.Visits')");
        }

        [Fact]
        public void JsonContainsPathAny()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContainsPathAny(e.CustomerJToken, "$.Statistics.Foo", "$.Statistics.Visits"));

            Assert.Equal(2, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS_PATH(`j`.`CustomerJToken`, 'one', '$.Statistics.Foo', '$.Statistics.Visits')");
        }

        [Fact]
        public void JsonContainsPathAll()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContainsPathAll(e.CustomerJToken, "$.Statistics.Foo", "$.Statistics.Visits"));

            Assert.Equal(0, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS_PATH(`j`.`CustomerJToken`, 'all', '$.Statistics.Foo', '$.Statistics.Visits')");
        }

        [Fact]
        public void JsonType()
        {
            using var ctx = CreateContext();
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonType(e.CustomerJToken["Statistics"]["Visits"]) == "INTEGER");

            Assert.Equal(2, count);
            AssertSql(
                @"SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_TYPE(JSON_EXTRACT(`j`.`CustomerJToken`, '$.Statistics.Visits')) = 'INTEGER'");
        }

        [Fact]
        public void JsonSearchAny()
        {
            using var ctx = CreateContext();
            var x = ctx.JsonEntities.Single(e => EF.Functions.JsonSearchAny(e.CustomerJToken, "J%", "$.Name"));

            Assert.Equal("Joe", x.CustomerJToken["Name"].Value<string>());
            AssertSql(
                @"SELECT `j`.`Id`, `j`.`CustomerJObject`, `j`.`CustomerJToken`
FROM `JsonEntities` AS `j`
WHERE JSON_SEARCH(`j`.`CustomerJToken`, 'one', 'J%', NULL, '$.Name') IS NOT NULL
LIMIT 2");
        }

//         [Fact]
//         public void JsonSearchAll()
//         {
//             using var ctx = CreateContext();
//             var count = ctx.JsonEntities.Count(e => EF.Functions.JsonSearchAll(e.CustomerJToken, "%o%"));
//
//             Assert.Equal(3, count);
//             AssertSql(
//                 $@"SELECT COUNT(*)
// FROM `JsonEntities` AS `j`
// WHERE JSON_SEARCH(`j`.`CustomerJToken`, 'all', '%o%') IS NOT NULL");
//         }
//
//         [Fact]
//         public void JsonSearchAll_with_path()
//         {
//             using var ctx = CreateContext();
//             var count = ctx.JsonEntities.Count(e => EF.Functions.JsonSearchAll(e.CustomerJToken, "%o%", "$.Name"));
//
//             Assert.Equal(2, count);
//             AssertSql(
//                 @"SELECT COUNT(*)
// FROM `JsonEntities` AS `j`
// WHERE JSON_SEARCH(`j`.`CustomerJToken`, 'all', '%o%', NULL, '$.Name') IS NOT NULL");
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

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        public class JsonDomQueryContext : PoolableDbContext
        {
            public DbSet<JsonEntity> JsonEntities { get; set; }

            public JsonDomQueryContext(DbContextOptions options) : base(options) {}

            public static void Seed(JsonDomQueryContext context)
            {
                var (customer1, customer2, customer3) = (createCustomer1(), createCustomer2(), createCustomer3());

                context.JsonEntities.AddRange(
                    new JsonEntity { Id = 1, CustomerJObject = customer1, CustomerJToken = customer1},
                    new JsonEntity { Id = 2, CustomerJObject = customer2, CustomerJToken = customer2},
                    new JsonEntity { Id = 3, CustomerJObject = null, CustomerJToken = customer3});
                context.SaveChanges();

                static JObject createCustomer1() => JObject.Parse(@"
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

                static JObject createCustomer2() => JObject.Parse(@"
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

                static JToken createCustomer3() => JToken.Parse(@"""foo""");
            }
        }

        public class JsonEntity
        {
            public int Id { get; set; }

            public JObject CustomerJObject { get; set; }
            public JToken CustomerJToken { get; set; }
        }

        public class JsonNewtonsoftDomQueryFixture : SharedStoreFixtureBase<JsonDomQueryContext>
        {
            protected override string StoreName => "JsonNewtonsoftDomQueryTest";
            protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;
            public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;
            protected override void Seed(JsonDomQueryContext context) => JsonDomQueryContext.Seed(context);

            protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
            {
                return base.AddServices(serviceCollection)
                    .AddEntityFrameworkMySqlJsonNewtonsoft();
            }

            public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
            {
                var options = base.AddOptions(builder);
                new MySqlDbContextOptionsBuilder(options)
                    .UseNewtonsoftJson();

                return options;
            }
        }

        #endregion
    }
}
