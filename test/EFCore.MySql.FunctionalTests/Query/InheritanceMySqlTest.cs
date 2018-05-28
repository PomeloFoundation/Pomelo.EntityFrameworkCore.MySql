using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.TestModels.Inheritance;
using Xunit;
using Xunit.Abstractions;

namespace EFCore.MySql.UpstreamFunctionalTests.Query
{
    public class InheritanceMySqlTest : InheritanceRelationalTestBase<InheritanceMySqlFixture>
    {
        public InheritanceMySqlTest(InheritanceMySqlFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }


        [Fact]
        public override void FromSql_on_root()
        {
            using (var context = CreateContext())
            {
                context.Set<Animal>().FromSql(@"select * from `Animal`").ToList();
            }
        }

        [Fact]
        public override void FromSql_on_derived()
        {
            using (var context = CreateContext())
            {
                context.Set<Eagle>().FromSql(@"select * from `Animal`").ToList();
            }
        }

        protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
            => facade.UseTransaction(transaction.GetDbTransaction());
    }
}
