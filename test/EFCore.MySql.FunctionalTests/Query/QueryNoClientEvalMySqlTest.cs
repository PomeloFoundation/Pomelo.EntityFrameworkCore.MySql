using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class QueryNoClientEvalMySqlTest : QueryNoClientEvalTestBase<QueryNoClientEvalMySqlFixture>
    {
        public QueryNoClientEvalMySqlTest(QueryNoClientEvalMySqlFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public override void Doesnt_throw_when_from_sql_not_composed()
        {
            using (var context = CreateContext())
            {
                var customers
                    = context.Customers
                        .FromSql(@"select * from `Customers`")
                        .ToList();

                Assert.Equal(91, customers.Count);
            }
        }
    }
}
