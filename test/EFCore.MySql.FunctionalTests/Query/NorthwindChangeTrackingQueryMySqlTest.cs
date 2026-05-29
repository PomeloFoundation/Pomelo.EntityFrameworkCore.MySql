using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestModels.Northwind;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindChangeTrackingQueryMySqlTest : NorthwindChangeTrackingQueryTestBase<
        NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {

        [ConditionalFact(Skip = "EF Core 10 emits an expression (e.g. LEAST) in LIMIT; MySQL only allows literals/parameters")]
        public override void Entity_range_does_not_revert_when_attached_dbSet() { }

        [ConditionalFact(Skip = "EF Core 10 emits an expression (e.g. LEAST) in LIMIT; MySQL only allows literals/parameters")]
        public override void Entity_range_does_not_revert_when_attached_dbContext() { }
        public NorthwindChangeTrackingQueryMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        protected override NorthwindContext CreateNoTrackingContext()
            => new NorthwindMySqlContext(
                new DbContextOptionsBuilder(Fixture.CreateOptions())
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options);
    }
}
