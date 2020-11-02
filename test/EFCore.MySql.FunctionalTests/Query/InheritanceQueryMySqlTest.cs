using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class InheritanceQueryMySqlTest : InheritanceRelationalQueryTestBase<InheritanceQueryMySqlFixture>
    {
        public InheritanceQueryMySqlTest(InheritanceQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory(Skip = "https://github.com/mysql-net/MySqlConnector/pull/896")]
        public override Task Byte_enum_value_constant_used_in_projection(bool async)
        {
            return base.Byte_enum_value_constant_used_in_projection(async);
        }

        public override void Setting_foreign_key_to_a_different_type_throws()
        {
            base.Setting_foreign_key_to_a_different_type_throws();
        }
    }
}
