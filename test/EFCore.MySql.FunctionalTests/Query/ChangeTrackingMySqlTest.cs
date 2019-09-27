using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class ChangeTrackingMySqlTest : ChangeTrackingTestBase<NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public ChangeTrackingMySqlTest(NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture)
            : base(fixture)
        {
        }

        protected override NorthwindContext CreateNoTrackingContext()
            => new NorthwindRelationalContext(
                new DbContextOptionsBuilder(Fixture.CreateOptions())
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).Options);
    }
}
