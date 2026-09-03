using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class FromSqlQueryMySqlTest : FromSqlQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public FromSqlQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.CommonTableExpressions))]
        public override Task FromSqlRaw_composed_with_common_table_expression(bool async)
        {
            return base.FromSqlRaw_composed_with_common_table_expression(async);
        }

        protected override DbParameter CreateDbParameter(string name, object value)
            => new MySqlParameter
            {
                ParameterName = name,
                Value = value
            };
    
        [ConditionalTheory(Skip = "Reusing the same DbParameter instance across multiple FromSql occurrences behaves differently with MySqlConnector.")]
        [MemberData(nameof(IsAsyncData))]
        public override Task Multiple_occurrences_of_FromSql_with_db_parameter_adds_two_parameters(bool async)
            => Task.CompletedTask;

}
}
