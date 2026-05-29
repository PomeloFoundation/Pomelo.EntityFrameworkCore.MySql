using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microting.EntityFrameworkCore.MySql.Infrastructure;
using Microting.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit.Abstractions;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    // Re-enabled to test JSON string query functionality using Microsoft System.Text.Json
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.Json))]
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTableImplementationStable))]
    public class JsonMicrosoftStringQueryTest : JsonStringQueryTestBase<JsonMicrosoftStringQueryTest.JsonMicrosoftStringQueryFixture>
    {
        public JsonMicrosoftStringQueryTest(JsonMicrosoftStringQueryFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public class JsonMicrosoftStringQueryFixture : JsonStringQueryFixtureBase
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
