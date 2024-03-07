using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class NorthwindQueryFiltersQueryMySqlTest : NorthwindQueryFiltersQueryTestBase<
        NorthwindQueryMySqlFixture<NorthwindQueryFiltersCustomizer>>
    {
        public NorthwindQueryFiltersQueryMySqlTest(
            NorthwindQueryMySqlFixture<NorthwindQueryFiltersCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        public override async Task Count_query(bool async)
        {
            await base.Count_query(async);

        AssertSql(
"""
@__ef_filter__TenantPrefix_0_startswith='B%' (Size = 40)

SELECT COUNT(*)
FROM `Customers` AS `c`
WHERE `c`.`CompanyName` LIKE @__ef_filter__TenantPrefix_0_startswith
""");
        }

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
    }
}
