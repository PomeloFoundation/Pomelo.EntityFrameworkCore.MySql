using System.Data.Common;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySql.Data.MySqlClient;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
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
    }
}
