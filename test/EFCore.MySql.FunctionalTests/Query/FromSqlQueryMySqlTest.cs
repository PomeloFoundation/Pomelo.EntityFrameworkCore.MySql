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
    }
}
