using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.Json))]
    public class JsonMicrosoftPocoQueryTest : JsonPocoQueryTestBase<JsonMicrosoftPocoQueryTest.JsonMicrosoftPocoQueryFixture>
    {
        public JsonMicrosoftPocoQueryTest(JsonMicrosoftPocoQueryFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [Fact]
        public override void JsonContains_with_json_element()
        {
            using var ctx = CreateContext();
            var element = JsonDocument.Parse(@"{""Name"": ""Joe"", ""Age"": 25}").RootElement;
            var count = ctx.JsonEntities.Count(e =>
                EF.Functions.JsonContains(e.Customer, element));

            Assert.Equal(1, count);
            AssertSql(
                $@"@__element_1='{{""Name"":""Joe"",""Age"":25}}' (Nullable = false) (Size = 4000)

SELECT COUNT(*)
FROM `JsonEntities` AS `j`
WHERE JSON_CONTAINS(`j`.`Customer`, {InsertJsonConvert("@__element_1")})");
        }

        public class JsonMicrosoftPocoQueryFixture : JsonPocoQueryFixtureBase
        {
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
    }
}
