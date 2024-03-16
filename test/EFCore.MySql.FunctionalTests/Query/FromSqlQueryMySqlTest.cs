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

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.ExceptIntercept))]
        public override Task Multiple_occurrences_of_FromSql_with_db_parameter_adds_parameter_only_once(bool async)
        {
            return base.Multiple_occurrences_of_FromSql_with_db_parameter_adds_parameter_only_once(async);
        }

        protected override DbParameter CreateDbParameter(string name, object value)
            => new MySqlParameter
            {
                ParameterName = name,
                Value = value
            };
    }
}
