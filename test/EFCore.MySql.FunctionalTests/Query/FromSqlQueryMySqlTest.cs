using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySqlConnector;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class FromSqlQueryMySqlTest : FromSqlQueryTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public FromSqlQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }

        protected override DbParameter CreateDbParameter(string name, object value)
            => new MySqlParameter
            {
                ParameterName = name,
                Value = value
            };

        [ConditionalTheory(Skip = "https://github.com/dotnet/efcore/pull/24806")]
        public override Task FromSqlRaw_in_subquery_with_dbParameter(bool async)
            => base.FromSqlRaw_in_subquery_with_dbParameter(async);

        [ConditionalTheory(Skip = "https://github.com/dotnet/efcore/pull/24806")]
        public override Task FromSqlRaw_in_subquery_with_positional_dbParameter_with_name(bool async)
            => base.FromSqlRaw_in_subquery_with_positional_dbParameter_with_name(async);

        [ConditionalTheory(Skip = "https://github.com/dotnet/efcore/pull/24806")]
        public override Task FromSqlRaw_in_subquery_with_positional_dbParameter_without_name(bool async)
            => base.FromSqlRaw_in_subquery_with_positional_dbParameter_without_name(async);

        [ConditionalTheory(Skip = "https://github.com/dotnet/efcore/pull/24806")]
        public override Task FromSqlRaw_with_dbParameter_mixed_in_subquery(bool async)
            => base.FromSqlRaw_with_dbParameter_mixed_in_subquery(async);
    }
}
