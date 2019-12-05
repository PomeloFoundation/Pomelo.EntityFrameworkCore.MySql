using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

        [ConditionalFact]
        public override void Doesnt_throw_when_from_sql_not_composed()
        {
            using (var context = CreateContext())
            {
                var customers
                    = context.Customers
                        .FromSqlRaw(@"select * from `Customers`")
                        .ToList();

                Assert.Equal(91, customers.Count);
            }
        }

        [ConditionalFact]
        public override void Throws_when_from_sql_composed()
        {
            using (var context = CreateContext())
            {
                Assert.Equal(
                    CoreStrings.TranslationFailed(@"DbSet<Customer>    .FromSqlOnQueryable(        source: ""select * from `Customers`"",         sql: __p_0)    .Where(c => c.IsLondon)"),
                    RemoveNewLines(Assert.Throws<InvalidOperationException>(
                        () => context.Customers
                            .FromSqlRaw(NormalizeDelimetersInRawString("select * from [Customers]"))
                            .Where(c => c.IsLondon)
                            .ToList()).Message));
            }
        }

        private string RemoveNewLines(string message)
            => message.Replace("\n", "").Replace("\r", "");
 
        private string NormalizeDelimetersInRawString(string sql)
            => Fixture.TestStore.NormalizeDelimitersInRawString(sql);
    }
}
