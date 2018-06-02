using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities.Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class AsyncGearsOfWarQueryMySqlTest : AsyncGearsOfWarQueryTestBase<GearsOfWarQueryMySqlFixture>
    {
        public AsyncGearsOfWarQueryMySqlTest(GearsOfWarQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalFact(Skip = "issue #552")]
        public override Task Enum_ToString_is_client_eval()
        {
            return base.Enum_ToString_is_client_eval();
        }

        [ConditionalFact(Skip = "issue #571")]
        public override Task Projecting_nullable_bool_in_conditional_works()
        {
            return base.Projecting_nullable_bool_in_conditional_works();
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
